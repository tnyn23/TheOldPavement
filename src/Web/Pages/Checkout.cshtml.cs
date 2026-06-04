using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Helpers;

namespace Web.Pages;

public sealed class CheckoutModel : PageModel
{
    private readonly ICheckoutService _checkoutService;
    private readonly IUserRepository _userRepository;
    private readonly IMomoService _momoService;
    private readonly TheOldPavementDbContext _db;
    private readonly ILogger<CheckoutModel> _logger;

    public List<CartItemDTO> CartItems { get; set; } = new();
    public decimal TotalPrice { get; set; }

    [BindProperty] public int? PromoCodeId { get; set; }
    [BindProperty] public string FullName { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public string Address { get; set; } = string.Empty;
    [BindProperty] public string City { get; set; } = string.Empty;
    [BindProperty] public string? District { get; set; }
    [BindProperty] public string? Ward { get; set; }
    [BindProperty] public string? Note { get; set; }
    [BindProperty] public string PaymentMethod { get; set; } = "cod";

    public CheckoutModel(
        ICheckoutService checkoutService,
        IUserRepository userRepository,
        IMomoService momoService,
        TheOldPavementDbContext db,
        ILogger<CheckoutModel> logger)
    {
        _checkoutService = checkoutService;
        _userRepository  = userRepository;
        _momoService     = momoService;
        _db              = db;
        _logger          = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        CartItems  = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);

        if (CartItems.Count == 0)
            return RedirectToPage("/Cart");

        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int uid))
            {
                var user = await _userRepository.GetByIdAsync(uid);
                if (user != null)
                {
                    FullName = user.FullName ?? string.Empty;
                    Email    = user.Email;
                    Phone    = user.Phone ?? string.Empty;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitOrderAsync()
    {
        CartItems  = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);

        if (CartItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Giỏ hàng của bạn đang trống.");
            return Page();
        }

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Phone)    || string.IsNullOrWhiteSpace(Address) ||
            string.IsNullOrWhiteSpace(City))
        {
            ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin giao hàng bắt buộc.");
            return Page();
        }

        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int uid))
                userId = uid;
        }

        var request = new CheckoutRequestDTO
        {
            UserId        = userId,
            FullName      = FullName,
            Email         = Email,
            Phone         = Phone,
            Address       = Address,
            City          = City,
            District      = District,
            Ward          = Ward,
            Note          = Note,
            PaymentMethod = PaymentMethod,
            PromoCodeId   = PromoCodeId,
            CartItems     = CartItems,
            TotalPrice    = TotalPrice
        };

        var result = await _checkoutService.ProcessCheckoutAsync(request);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        // ── MoMo ────────────────────────────────────────────────────────────
        if (PaymentMethod.Equals("momo", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage(
                "/Public/Payment/BankTransfer",
                new { orderNumber = result.OrderNumber, amount = result.TotalAmount, method = "momo" });
        }

        // ── Bank transfer ────────────────────────────────────────────────────
        if (PaymentMethod.Equals("bank", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToPage(
                "/Public/Payment/BankTransfer",
                new { orderNumber = result.OrderNumber, amount = result.TotalAmount, method = "bank" });
        }

        // ── COD ──────────────────────────────────────────────────────────────
        CartManager.ClearCart(HttpContext.Session);
        TempData["OrderedNumber"] = result.OrderNumber;
        return RedirectToPage("/ThankYouCard");
    }

    public async Task<IActionResult> OnPostApplyCouponAsync([FromBody] ApplyCouponRequest request)
    {
        var cartItems = CartManager.GetCart(HttpContext.Session);
        
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int uid))
                userId = uid;
        }

        var result = await _checkoutService.ValidatePromoCodeAsync(request.Code, request.Subtotal, userId, cartItems);
        return new JsonResult(new
        {
            success        = result.Success,
            promoId        = result.PromoId,
            code           = result.Code,
            discountAmount = result.DiscountAmount,
            finalTotal     = result.FinalTotal,
            message        = result.Message
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // PRIVATE
    // ─────────────────────────────────────────────────────────────────────────

    private async Task<IActionResult> HandleMomoPaymentAsync(CheckoutResultDTO result)
    {
        try
        {
            var order = await _db.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == result.OrderNumber);

            if (order is null)
            {
                _logger.LogError("Order {OrderNumber} not found after checkout", result.OrderNumber);
                ModelState.AddModelError(string.Empty, "Không tìm thấy đơn hàng vừa tạo.");
                return Page();
            }

            var baseUrl     = $"{Request.Scheme}://{Request.Host}";
            // returnUrl — browser redirect (user-facing result page)
            var returnUrl   = $"{baseUrl}/Public/Payment/MomoCallback";
            // ipnUrl — server-to-server notification (authoritative)
            var ipnUrl      = $"{baseUrl}/Public/Payment/MomoCallback?ipn=true";

            var momoResult = await _momoService.CreatePaymentAsync(order, returnUrl, ipnUrl);

            if (!momoResult.Success)
            {
                _logger.LogWarning(
                    "MoMo payment creation failed for order {OrderNumber}: {Error}",
                    result.OrderNumber, momoResult.ErrorMessage);

                // Fallback to simulate page in dev/test environment
                var simulateUrl = $"/Public/Payment/MomoSimulate" +
                    $"?orderId={Uri.EscapeDataString(result.OrderNumber)}" +
                    $"&amount={((long)Math.Round(result.TotalAmount))}" +
                    $"&orderInfo={Uri.EscapeDataString($"Thanh toan don hang {result.OrderNumber}")}";

                return Redirect(simulateUrl);
            }

            return Redirect(momoResult.PayUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating MoMo payment for order {OrderNumber}", result.OrderNumber);
            ModelState.AddModelError(string.Empty, $"Lỗi khởi tạo thanh toán MoMo: {ex.Message}");
            return Page();
        }
    }
}

public sealed class ApplyCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
}
