using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Application.DTOs;
using TheOldPavement.Application.Interfaces;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;
using TheOldPavement.Web.Helpers;
using TheOldPavement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;


namespace TheOldPavement.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly TheOldPavementDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IMomoService _momoService;

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
        IOrderRepository orderRepository, 
        IUserRepository userRepository, 
        TheOldPavementDbContext context, 
        IEmailService emailService,
        IMomoService momoService)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _context = context;
        _emailService = emailService;
        _momoService = momoService;
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
        else
        {
            // Guest checkout: Check if email already has an account
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email.ToLower().Trim());
            if (existingUser != null)
            {
                // Link order to existing account
                userId = existingUser.Id;
            }
            else
            {
                // Create a background account automatically
                var rawPassword = "TOP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
                var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawPassword)));

                var newUser = new User
                {
                    Email = Email.ToLower().Trim(),
                    FullName = FullName,
                    Phone = Phone,
                    Role = "customer",
                    PasswordHash = hash,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                userId = newUser.Id;

                // Send account creation email in the background (fire and forget)
                _ = Task.Run(() => _emailService.SendAccountCreationEmailAsync(Email.ToLower().Trim(), FullName, rawPassword));
            }
        }

        // Recalculate discount amount securely on the backend if a promo code is applied
        decimal discountAmount = 0;
        if (PromoCodeId.HasValue)
        {
            var promo = await _context.PromoCodes.FindAsync(PromoCodeId.Value);
            if (promo != null && promo.IsActive == true && promo.StartDate <= DateTime.Now && promo.EndDate >= DateTime.Now)
            {
                // Check usage limit
                if (!promo.UsageLimit.HasValue || (promo.UsedCount ?? 0) < promo.UsageLimit.Value)
                {
                    // Check minimum order value
                    if (!promo.MinOrderValue.HasValue || TotalPrice >= promo.MinOrderValue.Value)
                    {
                        if (promo.Type == "percent")
                        {
                            discountAmount = TotalPrice * (promo.Value / 100);
                            if (promo.MaxDiscount.HasValue && discountAmount > promo.MaxDiscount.Value)
                            {
                                discountAmount = promo.MaxDiscount.Value;
                            }
                        }
                        else if (promo.Type == "fixed")
                        {
                            discountAmount = promo.Value;
                        }

                        if (discountAmount > TotalPrice)
                        {
                            discountAmount = TotalPrice;
                        }

                        // Increment used count
                        promo.UsedCount = (promo.UsedCount ?? 0) + 1;
                        promo.UpdatedAt = DateTime.Now;
                        _context.PromoCodes.Update(promo);
                    }
                }
            }
        }

        // Create new Order model in EF Core
        var order = new Order
        {
            OrderNumber = "TOP" + DateTime.Now.ToString("yyyyMMdd") + new Random().Next(1000, 9999),
            UserId = userId,
            Status = "pending",
            Subtotal = TotalPrice,
            ShippingFee = 0,
            DiscountAmount = discountAmount,
            TotalAmount = TotalPrice - discountAmount,
            PromoCodeId = PromoCodeId,
            PaymentMethod = PaymentMethod,
            PaymentStatus = "pending",
            Note = Note,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            OrderItems = CartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                ProductName = item.ProductName,
                Size = item.Size,
                Color = item.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.TotalPrice,
                CreatedAt = DateTime.Now
            }).ToList(),
            ShippingAddress = new ShippingAddress
            {
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Address = Address,
                City = City,
                District = District ?? string.Empty,
                Ward = Ward ?? string.Empty,
                CreatedAt = DateTime.Now
            }
        };

        // Save order inside database using EF Core
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        if (PaymentMethod.ToLower() == "momo")
        {
            try
            {
                // Redirect to our local MoMo simulation page for testing
                var amount = ((long)Math.Round(order.TotalAmount)).ToString();
                var orderInfo = Uri.EscapeDataString($"Thanh toan don hang {order.OrderNumber} qua MoMo");
                var simulateUrl = $"/Public/Payment/MomoSimulate?orderId={order.OrderNumber}&amount={amount}&orderInfo={orderInfo}";
                
                return Redirect(simulateUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khởi tạo cổng thanh toán MoMo: {ex.Message}");
                
                // Rollback order from DB to let the user try again
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Page();
            }
        }

        // Send order confirmation email in the background (fire and forget)
        _ = Task.Run(() => _emailService.SendOrderConfirmationEmailAsync(order));

        // Clear local cart
        CartManager.ClearCart(HttpContext.Session);

        TempData["OrderedNumber"] = order.OrderNumber;
        return RedirectToPage("/ThankYouCard");
    }

    public async Task<IActionResult> OnPostApplyCouponAsync([FromBody] ApplyCouponRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Code))
        {
            return new JsonResult(new { success = false, message = "Vui lòng nhập mã giảm giá." });
        }

        var promo = await _context.PromoCodes
            .FirstOrDefaultAsync(p => p.Code == request.Code.ToUpper() && p.IsActive == true);

        if (promo == null)
        {
            return new JsonResult(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã bị vô hiệu hóa." });
        }

        if (promo.StartDate > DateTime.Now)
        {
            return new JsonResult(new { success = false, message = "Mã giảm giá chưa đến thời gian áp dụng." });
        }

        if (promo.EndDate < DateTime.Now)
        {
            return new JsonResult(new { success = false, message = "Mã giảm giá đã hết hạn sử dụng." });
        }

        if (promo.UsageLimit.HasValue && (promo.UsedCount ?? 0) >= promo.UsageLimit.Value)
        {
            return new JsonResult(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng." });
        }

        if (promo.MinOrderValue.HasValue && request.Subtotal < promo.MinOrderValue.Value)
        {
            return new JsonResult(new { success = false, message = $"Đơn hàng chưa đạt giá trị tối thiểu {promo.MinOrderValue.Value:N0}₫ để áp dụng mã này." });
        }

        decimal discount = 0;
        if (promo.Type == "percent")
        {
            discount = request.Subtotal * (promo.Value / 100);
            if (promo.MaxDiscount.HasValue && discount > promo.MaxDiscount.Value)
            {
                discount = promo.MaxDiscount.Value;
            }
        }
        else if (promo.Type == "fixed")
        {
            discount = promo.Value;
        }

        if (discount > request.Subtotal)
        {
            discount = request.Subtotal;
        }

        decimal finalTotal = request.Subtotal - discount;

        return new JsonResult(new { 
            success = true, 
            promoId = promo.Id, 
            code = promo.Code, 
            discountAmount = discount, 
            finalTotal = finalTotal,
            message = "Áp dụng mã giảm giá thành công!" 
        });
    }
}

public class ApplyCouponRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
}

