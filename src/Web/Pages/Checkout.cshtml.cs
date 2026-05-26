using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Web.Helpers;

namespace Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly ICheckoutService _checkoutService;
    private readonly IUserRepository _userRepository;

    public List<CartItemDTO> CartItems { get; set; } = new();
    public decimal TotalPrice { get; set; }

    [BindProperty]
    public int? PromoCodeId { get; set; }


    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    public string Address { get; set; } = string.Empty;

    [BindProperty]
    public string City { get; set; } = string.Empty;

    [BindProperty]
    public string? District { get; set; }

    [BindProperty]
    public string? Ward { get; set; }

    [BindProperty]
    public string? Note { get; set; }

    [BindProperty]
    public string PaymentMethod { get; set; } = "cod";

    public CheckoutModel(
        ICheckoutService checkoutService,
        IUserRepository userRepository) // Keep for simple profile fetching on GET
    {
        _checkoutService = checkoutService;
        _userRepository = userRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        CartItems = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);

        if (CartItems.Count == 0)
        {
            return RedirectToPage("/Cart");
        }

        // Pre-fill user data if logged in
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    FullName = user.FullName ?? string.Empty;
                    Email = user.Email;
                    Phone = user.Phone ?? string.Empty;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSubmitOrderAsync()
    {
        CartItems = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);

        if (CartItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Giỏ hàng của bạn đang trống.");
            return Page();
        }

        if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Address) || string.IsNullOrEmpty(City))
        {
            ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ tất cả các trường thông tin giao hàng bắt buộc.");
            return Page();
        }

        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uId))
            {
                userId = uId;
            }
        }

        var request = new CheckoutRequestDTO
        {
            UserId = userId,
            FullName = FullName,
            Email = Email,
            Phone = Phone,
            Address = Address,
            City = City,
            District = District,
            Ward = Ward,
            Note = Note,
            PaymentMethod = PaymentMethod,
            PromoCodeId = PromoCodeId,
            CartItems = CartItems,
            TotalPrice = TotalPrice
        };

        var result = await _checkoutService.ProcessCheckoutAsync(request);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        if (PaymentMethod.ToLower() == "momo")
        {
            try
            {
                // Redirect to our local MoMo simulation page for testing
                var amount = ((long)Math.Round(result.TotalAmount)).ToString();
                var orderInfo = Uri.EscapeDataString($"Thanh toan don hang {result.OrderNumber} qua MoMo");
                var simulateUrl = $"/Public/Payment/MomoSimulate?orderId={result.OrderNumber}&amount={amount}&orderInfo={orderInfo}";
                
                return Redirect(simulateUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khởi tạo cổng thanh toán MoMo: {ex.Message}");
                return Page();
            }
        }

        // Clear local cart
        CartManager.ClearCart(HttpContext.Session);

        TempData["OrderedNumber"] = result.OrderNumber;
        return RedirectToPage("/ThankYouCard");
    }

    public async Task<IActionResult> OnPostApplyCouponAsync([FromBody] ApplyCouponRequest request)
    {
        var result = await _checkoutService.ValidatePromoCodeAsync(request.Code, request.Subtotal);

        return new JsonResult(new { 
            success = result.Success, 
            promoId = result.PromoId, 
            code = result.Code, 
            discountAmount = result.DiscountAmount, 
            finalTotal = result.FinalTotal,
            message = result.Message 
        });
    }
}

public class ApplyCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
}
