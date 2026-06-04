using System.Security.Cryptography;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<PromoCode> _promoRepository;
    private readonly IRepository<ProductVariant> _variantRepository;
    private readonly IProductRepository _productRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IRepository<PromoCode> promoRepository,
        IRepository<ProductVariant> variantRepository,
        IProductRepository productRepository,
        IEmailService emailService,
        ILogger<CheckoutService> logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _promoRepository = promoRepository;
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<PromoValidationResultDTO> ValidatePromoCodeAsync(string code, decimal subtotal, int? userId, List<CartItemDTO> cartItems)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new PromoValidationResultDTO { Success = false, Message = "Vui lòng nhập mã giảm giá." };
        }

        var promo = await _promoRepository
            .FirstOrDefaultAsync(p => p.Code == code.ToUpper() && p.IsActive == true);

        if (promo == null)
            return new PromoValidationResultDTO { Success = false, Message = "Mã giảm giá không hợp lệ hoặc đã bị vô hiệu hóa." };

        if (promo.StartDate > DateTime.Now)
            return new PromoValidationResultDTO { Success = false, Message = "Mã giảm giá chưa đến thời gian áp dụng." };

        if (promo.EndDate < DateTime.Now)
            return new PromoValidationResultDTO { Success = false, Message = "Mã giảm giá đã hết hạn sử dụng." };

        if (promo.UsageLimit.HasValue && (promo.UsedCount ?? 0) >= promo.UsageLimit.Value)
            return new PromoValidationResultDTO { Success = false, Message = "Mã giảm giá đã hết lượt sử dụng." };

        if (promo.MinOrderValue.HasValue && subtotal < promo.MinOrderValue.Value)
            return new PromoValidationResultDTO { Success = false, Message = $"Đơn hàng chưa đạt giá trị tối thiểu {promo.MinOrderValue.Value:N0}₫ để áp dụng mã này." };

        // VIP Tier check
        if (!string.IsNullOrWhiteSpace(promo.RequiredUserTier))
        {
            if (!userId.HasValue)
                return new PromoValidationResultDTO { Success = false, Message = $"Mã này chỉ dành cho khách hàng hạng {promo.RequiredUserTier}. Vui lòng đăng nhập." };
                
            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null || !string.Equals(user.Tier, promo.RequiredUserTier, StringComparison.OrdinalIgnoreCase))
                return new PromoValidationResultDTO { Success = false, Message = $"Mã này chỉ dành cho khách hàng hạng {promo.RequiredUserTier}." };
        }

        // Category check
        if (!string.IsNullOrWhiteSpace(promo.AppliesToCategory))
        {
            bool hasCategory = false;
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null && string.Equals(product.Category, promo.AppliesToCategory, StringComparison.OrdinalIgnoreCase))
                {
                    hasCategory = true;
                    break;
                }
            }
            if (!hasCategory)
                return new PromoValidationResultDTO { Success = false, Message = $"Mã này chỉ áp dụng cho danh mục {promo.AppliesToCategory}." };
        }

        // Combo / Product check
        if (!string.IsNullOrWhiteSpace(promo.AppliesToProductIds))
        {
            var requiredIds = promo.AppliesToProductIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(id => int.Parse(id.Trim())).ToList();
            
            var cartProductIds = cartItems.Select(c => c.ProductId).ToList();
            
            if (promo.IsCombo == true)
            {
                // Must have ALL required products
                bool hasAll = requiredIds.All(id => cartProductIds.Contains(id));
                if (!hasAll)
                    return new PromoValidationResultDTO { Success = false, Message = "Mã này chỉ áp dụng khi mua trọn bộ combo sản phẩm." };
            }
            else
            {
                // Must have AT LEAST ONE of the required products
                bool hasAny = requiredIds.Any(id => cartProductIds.Contains(id));
                if (!hasAny)
                    return new PromoValidationResultDTO { Success = false, Message = "Mã này không áp dụng cho các sản phẩm trong giỏ hàng." };
            }
        }

        // Buy X Get Y check
        if (promo.RequiredQuantity.HasValue && promo.RequiredQuantity.Value > 0)
        {
            int totalRelevantQuantity = 0;
            
            if (!string.IsNullOrWhiteSpace(promo.AppliesToCategory))
            {
                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null && string.Equals(product.Category, promo.AppliesToCategory, StringComparison.OrdinalIgnoreCase))
                    {
                        totalRelevantQuantity += item.Quantity;
                    }
                }
            }
            else
            {
                totalRelevantQuantity = cartItems.Sum(c => c.Quantity);
            }

            if (totalRelevantQuantity < promo.RequiredQuantity.Value)
            {
                return new PromoValidationResultDTO { Success = false, Message = $"Cần mua ít nhất {promo.RequiredQuantity.Value} sản phẩm để áp dụng mã này." };
            }
        }

        decimal discount = 0;
        if (promo.Type == "percent")
        {
            discount = subtotal * (promo.Value / 100);
            if (promo.MaxDiscount.HasValue && discount > promo.MaxDiscount.Value)
            {
                discount = promo.MaxDiscount.Value;
            }
        }
        else if (promo.Type == "fixed")
        {
            discount = promo.Value;
        }

        if (discount > subtotal)
        {
            discount = subtotal;
        }

        decimal finalTotal = subtotal - discount;

        return new PromoValidationResultDTO
        {
            Success = true,
            PromoId = promo.Id,
            Code = promo.Code,
            DiscountAmount = discount,
            FinalTotal = finalTotal,
            Message = "Áp dụng mã giảm giá thành công!"
        };
    }

    public async Task<CheckoutResultDTO> ProcessCheckoutAsync(CheckoutRequestDTO request)
    {
        if (request.CartItems == null || !request.CartItems.Any())
        {
            return new CheckoutResultDTO { Success = false, Message = "Giỏ hàng trống." };
        }

        int? finalUserId = request.UserId;

        // Handle Guest checkout (auto create account)
        if (!finalUserId.HasValue)
        {
            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());
            if (existingUser != null)
            {
                finalUserId = existingUser.Id;
            }
            else
            {
                var rawPassword = "TOP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
                var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawPassword)));

                var newUser = new User
                {
                    Email = request.Email.ToLower().Trim(),
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Role = "customer",
                    PasswordHash = hash,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync();
                finalUserId = newUser.Id;

                _ = Task.Run(() => _emailService.SendAccountCreationEmailAsync(request.Email.ToLower().Trim(), request.FullName, rawPassword));
            }
        }

        decimal discountAmount = 0;
        if (request.PromoCodeId.HasValue)
        {
            var promo = await _promoRepository.GetByIdAsync(request.PromoCodeId.Value);
            if (promo != null)
            {
                var validationResult = await ValidatePromoCodeAsync(promo.Code, request.TotalPrice, finalUserId, request.CartItems);
                if (validationResult.Success)
                {
                    discountAmount = validationResult.DiscountAmount;
                    promo.UsedCount = (promo.UsedCount ?? 0) + 1;
                    promo.UpdatedAt = DateTime.Now;
                    await _promoRepository.UpdateAsync(promo);
                }
                else
                {
                    return new CheckoutResultDTO { Success = false, Message = validationResult.Message };
                }
            }
        }

        // --- Inventory Check and Decrease ---
        foreach (var item in request.CartItems)
        {
            var variant = await _variantRepository.GetByIdAsync(item.VariantId);
            if (variant != null)
            {
                if (variant.StockQuantity.HasValue && variant.StockQuantity.Value < item.Quantity)
                {
                    return new CheckoutResultDTO { Success = false, Message = $"Sản phẩm {item.ProductName} ({item.Size}/{item.Color}) không đủ số lượng trong kho." };
                }
                
                if (variant.StockQuantity.HasValue)
                {
                    variant.StockQuantity -= item.Quantity;
                    if (variant.StockQuantity <= 0)
                    {
                        variant.StockQuantity = 0;
                        variant.IsAvailable = false;
                    }
                    await _variantRepository.UpdateAsync(variant);
                }
            }
        }

        // --- Calculate Shipping Fee ---
        decimal shippingFee = CalculateShippingFee(request.City, request.TotalPrice);

        var order = new Order
        {
            OrderNumber = "TOP" + DateTime.Now.ToString("yyyyMMdd") + new Random().Next(1000, 9999),
            UserId = finalUserId,
            Status = "pending",
            Subtotal = request.TotalPrice,
            ShippingFee = shippingFee,
            DiscountAmount = discountAmount,
            TotalAmount = request.TotalPrice - discountAmount + shippingFee,
            PromoCodeId = request.PromoCodeId,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = "pending",
            Note = request.Note,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            OrderItems = request.CartItems.Select(item => new OrderItem
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
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                District = request.District ?? string.Empty,
                Ward = request.Ward ?? string.Empty,
                CreatedAt = DateTime.Now
            }
        };

        await _orderRepository.AddAsync(order);
        
        // Save changes across all repositories via one SaveChanges on OrderRepository 
        // (which uses the shared DbContext underneath)
        await _orderRepository.SaveChangesAsync();

        // Send confirmation email — await directly so ShippingAddress navigation property is still in scope
        try
        {
            await _emailService.SendOrderConfirmationEmailAsync(order);
        }
        catch (Exception ex)
        {
            // Email failure should not block order completion
            _logger.LogError(ex, "[Email] Gửi email xác nhận đơn hàng {OrderNumber} thất bại: {Message}", order.OrderNumber, ex.Message);
        }

        return new CheckoutResultDTO 
        { 
            Success = true, 
            OrderNumber = order.OrderNumber, 
            TotalAmount = order.TotalAmount 
        };
    }

    /// <summary>
    /// Calculate shipping fee based on city.
    /// Free shipping for orders over 500,000₫.
    /// HCM/Hanoi: 30,000₫, Other cities: 45,000₫.
    /// </summary>
    private static decimal CalculateShippingFee(string city, decimal subtotal)
    {
        // Free shipping for orders over 500k
        if (subtotal >= 500000)
            return 0;

        var normalizedCity = city?.Trim().ToLower() ?? "";
        
        // Major cities: lower shipping fee
        var majorCities = new[] { "hồ chí minh", "ho chi minh", "hcm", "tp.hcm", "tp hcm", "hà nội", "ha noi", "hanoi" };
        
        if (majorCities.Any(c => normalizedCity.Contains(c)))
            return 30000;

        return 45000;
    }
}
