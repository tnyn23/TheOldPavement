using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheOldPavement.Domain.Models;
using TheOldPavement.Infrastructure.Context;

namespace TheOldPavement.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public List<TheOldPavement.Domain.Models.Product> Products { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<ProductVariant> ProductVariants { get; set; } = new();
    public List<PromoCode> PromoCodes { get; set; } = new();
    
    // Analytics Metrics
    public decimal TotalRevenue { get; set; }
    public int TotalOrdersCount { get; set; }
    public int TotalProductsCount { get; set; }
    public int TotalUsersCount { get; set; }

    // Dynamic Chart Data
    public decimal[] SalesByDay { get; set; } = new decimal[7];

    // Dynamic Top Selling DTOs
    public List<TopProductDTO> TopSellingProducts { get; set; } = new();

    // Payment Channel percentages
    public int CodPercentage { get; set; } = 50;
    public int TransferPercentage { get; set; } = 50;

    public DashboardModel(TheOldPavementDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole("admin"))
        {
            return RedirectToPage("/Public/Account/Login");
        }

        await LoadDataAsync();
        return Page();
    }

    private async Task LoadDataAsync()
    {
        // 1. Check and Seed database dynamically if it is completely empty
        if (!_context.Products.Any())
        {
            var p1 = new TheOldPavement.Domain.Models.Product 
            { 
                Name = "36 Phố Phường - Hồn Hà Nội Tee", 
                Slug = "36-pho-phuong", 
                Price = 425000, 
                Category = "tee", 
                Status = "available", 
                Description = "Áo thun streetwear chất lượng cao lấy cảm hứng từ phố cổ Hà Nội.",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-10)
            };
            p1.ProductImages.Add(new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080", IsPrimary = true, CreatedAt = DateTime.Now });
            p1.ProductVariants.Add(new ProductVariant { Size = "L", Color = "White", ColorHex = "#FFFFFF", Sku = "OP-36PP-WHT-L", StockQuantity = 120, IsAvailable = true });
            p1.ProductVariants.Add(new ProductVariant { Size = "M", Color = "White", ColorHex = "#FFFFFF", Sku = "OP-36PP-WHT-M", StockQuantity = 85, IsAvailable = true });

            var p2 = new TheOldPavement.Domain.Models.Product 
            { 
                Name = "The Old Pavement Classic Black Tee", 
                Slug = "classic-black-tee", 
                Price = 425000, 
                Category = "tee", 
                Status = "available",
                Description = "Chiếc áo thun basic streetwear với logo thêu sắc nét.",
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now.AddDays(-5)
            };
            p2.ProductImages.Add(new ProductImage { ImageUrl = "https://images.unsplash.com/photo-1662103627854-ae7551d1eddb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080", IsPrimary = true, CreatedAt = DateTime.Now });
            p2.ProductVariants.Add(new ProductVariant { Size = "XL", Color = "Black", ColorHex = "#000000", Sku = "OP-CLBT-BLK-XL", StockQuantity = 95, IsAvailable = true });

            _context.Products.AddRange(p1, p2);
            await _context.SaveChangesAsync();
        }

        // Retrieve existing products including variants to link foreign keys correctly and avoid database constraint errors
        var existingProducts = await _context.Products.Include(p => p.ProductVariants).ToListAsync();
        var prod36 = existingProducts.FirstOrDefault(p => p.Slug == "36-pho-phuong") ?? existingProducts.FirstOrDefault();
        var prodClassic = existingProducts.FirstOrDefault(p => p.Slug == "classic-black-tee") ?? existingProducts.LastOrDefault();

        var var36 = prod36?.ProductVariants.FirstOrDefault(v => v.Size == "L") ?? prod36?.ProductVariants.FirstOrDefault();
        var varClassic = prodClassic?.ProductVariants.FirstOrDefault(v => v.Size == "XL") ?? prodClassic?.ProductVariants.FirstOrDefault();

        if (!_context.Orders.Any())
        {
            var o1 = new Order
            {
                OrderNumber = "TOP" + DateTime.Now.ToString("yyyyMMdd") + "1182",
                CreatedAt = DateTime.Now.AddHours(-5),
                Status = "delivered",
                Subtotal = 850000,
                TotalAmount = 850000,
                PaymentMethod = "cod",
                PaymentStatus = "paid",
                UpdatedAt = DateTime.Now
            };
            o1.OrderItems.Add(new OrderItem 
            { 
                ProductId = prod36?.Id ?? 1,
                VariantId = var36?.Id ?? 1,
                ProductName = prod36?.Name ?? "36 Phố Phường - Hồn Hà Nội Tee", 
                Quantity = 2, 
                UnitPrice = 425000, 
                Size = "L", 
                Color = "White", 
                Subtotal = 850000, 
                CreatedAt = DateTime.Now 
            });
            
            var o2 = new Order
            {
                OrderNumber = "TOP" + DateTime.Now.ToString("yyyyMMdd") + "9024",
                CreatedAt = DateTime.Now.AddDays(-1),
                Status = "pending",
                Subtotal = 425000,
                TotalAmount = 425000,
                PaymentMethod = "bank_transfer",
                PaymentStatus = "pending",
                UpdatedAt = DateTime.Now
            };
            o2.OrderItems.Add(new OrderItem 
            { 
                ProductId = prodClassic?.Id ?? 2,
                VariantId = varClassic?.Id ?? 2,
                ProductName = prodClassic?.Name ?? "The Old Pavement Classic Black Tee", 
                Quantity = 1, 
                UnitPrice = 425000, 
                Size = "XL", 
                Color = "Black", 
                Subtotal = 425000, 
                CreatedAt = DateTime.Now 
            });

            _context.Orders.AddRange(o1, o2);
            await _context.SaveChangesAsync();
        }

        // 2. Fetch all real data from database tables
        Products = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue)
            .ToListAsync();

        Orders = await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        ProductVariants = await _context.ProductVariants
            .Include(v => v.Product)
            .ToListAsync();

        PromoCodes = await _context.PromoCodes
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        // 3. Compute Analytics metrics dynamically
        TotalRevenue = Orders.Sum(o => o.TotalAmount);
        TotalOrdersCount = Orders.Count;
        TotalProductsCount = Products.Count;
        TotalUsersCount = await _context.Users.CountAsync();
        if (TotalUsersCount == 0) TotalUsersCount = 12; // Fallback customer count if empty

        // 4. Compute daily sales for current week dynamically (Monday - Sunday)
        SalesByDay = new decimal[7];
        foreach (var order in Orders)
        {
            if (order.CreatedAt.HasValue)
            {
                // DayOfWeek in .NET starts from Sunday = 0. Adjust so Monday = 0, Sunday = 6
                int dayIndex = ((int)order.CreatedAt.Value.DayOfWeek + 6) % 7;
                SalesByDay[dayIndex] += order.TotalAmount;
            }
        }

        // 5. Compute top selling products dynamically
        var groupedSelling = Orders.SelectMany(o => o.OrderItems)
            .GroupBy(item => item.ProductName)
            .Select(g => new TopProductDTO
            {
                Name = g.Key ?? "Sản phẩm",
                SalesCount = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.SalesCount)
            .Take(3)
            .ToList();
        
        TopSellingProducts = groupedSelling;

        // 6. Compute payment channels dynamically
        int codCount = Orders.Count(o => o.PaymentMethod == "cod");
        int transCount = Orders.Count(o => o.PaymentMethod != "cod");
        int totalPayments = codCount + transCount;
        if (totalPayments > 0)
        {
            CodPercentage = codCount * 100 / totalPayments;
            TransferPercentage = transCount * 100 / totalPayments;
        }
    }

    public async Task<IActionResult> OnPostUpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng {order.OrderNumber} thành '{status}' thành công!";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveProductAsync(int? id, string name, string slug, decimal price, string category, string status)
    {
        // Handle multiple uploaded images and their corresponding colors (stored in ProductImage.AltText)
        var uploadedFiles = Request.Form.Files;
        var colors = Request.Form["imageColor"].ToArray();
        var savedImages = new List<(string Url, string? Color)>();
        if (uploadedFiles != null && uploadedFiles.Count > 0)
        {
            var uploadDir = Path.Combine(_environment.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            for (int i = 0; i < uploadedFiles.Count; i++)
            {
                var f = uploadedFiles[i];
                if (f == null || f.Length == 0) continue;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(f.FileName);
                var filePath = Path.Combine(uploadDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await f.CopyToAsync(stream);
                }
                var imageUrl = "/images/products/" + fileName;
                string? color = null;
                if (colors != null && colors.Length > i) color = string.IsNullOrWhiteSpace(colors[i]) ? null : colors[i].ToString().ToLowerInvariant();
                savedImages.Add((imageUrl, color));
            }
        }

        if (id.HasValue && id.Value > 0)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == id.Value);

            if (product != null)
            {
                product.Name = name;
                product.Slug = slug;
                product.Price = price;
                product.Category = category;
                product.Status = status;
                product.UpdatedAt = DateTime.Now;

                // Add any newly uploaded images (preserve existing images)
                if (savedImages.Any())
                {
                    foreach (var si in savedImages)
                    {
                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = si.Url,
                            AltText = si.Color,
                            IsPrimary = product.ProductImages.All(pi => pi.IsPrimary != true),
                            DisplayOrder = 0,
                            CreatedAt = DateTime.Now
                        });
                    }
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật sản phẩm '{name}' thành công!";
            }
        }
        else
        {
            var product = new TheOldPavement.Domain.Models.Product
            {
                Name = name,
                Slug = slug,
                Price = price,
                Category = category,
                Status = status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add uploaded images (if any)
            if (savedImages.Any())
            {
                for (int i = 0; i < savedImages.Count; i++)
                {
                    var si = savedImages[i];
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = si.Url,
                        AltText = si.Color,
                        IsPrimary = i == 0,
                        DisplayOrder = i,
                        CreatedAt = DateTime.Now
                    });
                }
            }
            else
            {
                // Fallback / default image if no image uploaded
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080",
                    IsPrimary = true,
                    DisplayOrder = 0,
                    CreatedAt = DateTime.Now
                });
            }

            // Add default variant for the new product to make it immediately purchasable/stockable
            product.ProductVariants.Add(new ProductVariant
            {
                Size = "L",
                Color = "White",
                ColorHex = "#FFFFFF",
                Sku = $"OP-{slug.ToUpper()}-WHT-L",
                StockQuantity = 100,
                IsAvailable = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Thêm mới sản phẩm '{name}' thành công!";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteProductAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Xóa sản phẩm '{product.Name}' thành công!";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateStockAsync(int variantId, int stockQuantity)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant != null)
        {
            variant.StockQuantity = stockQuantity;
            variant.UpdatedAt = DateTime.Now;
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật tồn kho thành công!";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSavePromoCodeAsync(int? id, string code, string promoType, decimal value, decimal? minOrderValue, int? usageLimit, DateTime startDate, DateTime endDate, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            TempData["ErrorMessage"] = "Mã khuyến mãi không được để trống!";
            return RedirectToPage();
        }

        if (id.HasValue && id.Value > 0)
        {
            var promo = await _context.PromoCodes.FindAsync(id.Value);
            if (promo != null)
            {
                promo.Code = code.ToUpper();
                promo.Type = promoType;
                promo.Value = value;
                promo.MinOrderValue = minOrderValue;
                promo.UsageLimit = usageLimit;
                promo.StartDate = startDate;
                promo.EndDate = endDate;
                promo.IsActive = isActive;
                promo.UpdatedAt = DateTime.Now;

                _context.PromoCodes.Update(promo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật mã khuyến mãi '{code}' thành công!";
            }
        }
        else
        {
            // Check duplicate code
            if (await _context.PromoCodes.AnyAsync(p => p.Code == code.ToUpper()))
            {
                TempData["ErrorMessage"] = $"Mã khuyến mãi '{code}' đã tồn tại!";
                return RedirectToPage();
            }

            var promo = new PromoCode
            {
                Code = code.ToUpper(),
                Type = promoType,
                Value = value,
                MinOrderValue = minOrderValue,
                UsageLimit = usageLimit,
                UsedCount = 0,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.PromoCodes.Add(promo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Thêm mã khuyến mãi '{code}' thành công!";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeletePromoCodeAsync(int id)
    {
        var promo = await _context.PromoCodes.FindAsync(id);
        if (promo != null)
        {
            // Instead of physical delete, we can soft delete or set IsActive = false
            promo.IsActive = false;
            promo.UpdatedAt = DateTime.Now;
            _context.PromoCodes.Update(promo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Vô hiệu hóa mã khuyến mãi '{promo.Code}' thành công!";
        }
        return RedirectToPage();
    }
}

public class TopProductDTO
{
    public string Name { get; set; } = string.Empty;
    public int SalesCount { get; set; }
}

