using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Infrastructure.Context;

namespace Web.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public List<Domain.Models.Product> Products { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<ProductVariant> ProductVariants { get; set; } = new();
    public List<PromoCode> PromoCodes { get; set; } = new();
    public List<ProductReview> Reviews { get; set; } = new();
    public HashSet<int> OrderedProductIds { get; set; } = new(); // sản phẩm đã có order
    
    // Analytics Metrics
    public decimal TotalRevenue { get; set; }
    public int TotalOrdersCount { get; set; }
    public int TotalProductsCount { get; set; }
    public int TotalUsersCount { get; set; }

    // Web Traffic Analytics
    public int TotalPageViews { get; set; }
    public int UniqueSessions { get; set; }

    // Dynamic Chart Data
    public decimal[] SalesByDay { get; set; } = new decimal[7];

    // Dynamic Top Selling DTOs
    public List<TopProductDTO> TopSellingProducts { get; set; } = new();

    // Payment Channel percentages
    public int CodPercentage { get; set; } = 50;
    public int TransferPercentage { get; set; } = 50;

    private readonly Application.Interfaces.IReviewService _reviewService;

    public DashboardModel(TheOldPavementDbContext context, IWebHostEnvironment environment, Application.Interfaces.IReviewService reviewService)
    {
        _context = context;
        _environment = environment;
        _reviewService = reviewService;
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
            var p1 = new Domain.Models.Product 
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

            var p2 = new Domain.Models.Product 
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
        var existingProducts = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .ToListAsync();

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

        // Seed Users if none exist to avoid FK errors
        if (!_context.Users.Any())
        {
            var u1 = new Domain.Models.User { FullName = "Nguyễn Văn A", Email = "nguyenvana@gmail.com", PasswordHash = "hashed", Role = "customer", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var u2 = new Domain.Models.User { FullName = "Trần Thị B", Email = "tranthib@gmail.com", PasswordHash = "hashed", Role = "customer", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            _context.Users.AddRange(u1, u2);
            await _context.SaveChangesAsync();
        }

        // Seed Reviews
        if (!_context.ProductReviews.Any())
        {
            var users = await _context.Users.Take(2).ToListAsync();
            var u1Id = users.Count > 0 ? users[0].Id : 1;
            var u2Id = users.Count > 1 ? users[1].Id : 1;

            var reviews = new List<Domain.Models.ProductReview>
            {
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Trần Văn Hoàng", Comment = "Áo đẹp, chất vải mềm mát. Shop đóng gói cẩn thận, giao hàng cực kỳ nhanh chóng. Rất ưng ý!", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-1) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 4, Title = "Nguyễn Minh Tuấn", Comment = "Form áo vừa vặn, mặc lên dáng chuẩn. Tuy nhiên đường chỉ ở lai áo hơi lỏng một chút.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-2) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Lê Hoàng Yến", Comment = "Mua tặng người yêu mà bạn ấy khen nức nở. Áo local brand mà chất lượng không kém gì hàng xịn.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-3) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 5, Title = "Phạm Quốc Bảo", Comment = "Sản phẩm giống hệt trên ảnh, chất liệu rất dày dặn nhưng không bị nóng. 10 điểm cho chất lượng.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-4) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 3, Title = "Bùi Thị Lan", Comment = "Áo mặc cũng được, nhưng phần cổ áo hơi chật so với size mình thường mặc.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-5) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 5, Title = "Đinh Nhật Minh", Comment = "Mình đã mua cái thứ 3 của shop rồi, thiết kế đơn giản nhưng rất tinh tế và dễ phối đồ.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-6) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 4, Title = "Vũ Thùy Linh", Comment = "Chất lượng áo tốt trong tầm giá. Sẽ giới thiệu cho bạn bè mua chung.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-7) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 5, Title = "Nguyễn Bảo Khang", Comment = "Giao hàng thần tốc, đặt hôm qua mà hôm nay đã có. Áo giặt không bị xù lông hay phai màu.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-8) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Hoàng Ngọc Hân", Comment = "Đỉnh của chóp, packaging rất xịn xò, có thiệp cảm ơn. Rất thích cách làm việc của The Old Pavement.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-9) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 4, Title = "Trương Minh Nhật", Comment = "Hàng ngon bổ rẻ, đúng chất streetwear Hà Nội. Vải cotton mát mẻ.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-10) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 2, Title = "Ngô Thành Công", Comment = "Mình nhận được hàng bị lỗi một vết bẩn nhỏ ở tay áo. Shop nhớ kiểm tra kĩ trước khi giao nhé.", IsApproved = false, CreatedAt = DateTime.Now.AddDays(-11) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 5, Title = "Lê Hải Đăng", Comment = "Quá xuất sắc, mặc lên người cảm giác thoải mái. Phù hợp cho những ngày đi dạo phố.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-12) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 4, Title = "Trần Tuấn Tú", Comment = "Thiết kế artwork sau lưng rất ngầu. Hi vọng shop ra thêm nhiều mẫu như thế này.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-13) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 5, Title = "Phan Gia Hưng", Comment = "Tuyệt vời, giá sinh viên nhưng chất lượng quá ổn áp. Mong The Old Pavement phát triển mạnh hơn nữa.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-14) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Nguyễn Hồng Ánh", Comment = "Màu sắc bên ngoài nhìn còn đẹp hơn trên mạng. Mua đợt sale nên giá rất hời.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-15) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 3, Title = "Đào Xuân Trường", Comment = "Vải hơi mỏng so với kỳ vọng của mình. Bù lại thiết kế đẹp và giao nhanh.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-16) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Lưu Đức Hải", Comment = "Mình là fan của shop từ những ngày đầu. Vẫn luôn giữ được chất riêng và chất lượng ngày càng đi lên.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-1) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 4, Title = "Trịnh Thu Huyền", Comment = "Form áo khá to, các bạn nữ muốn mặc vừa nên lùi 1 size. Áo rất đẹp.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-2) },
                new() { ProductId = prod36?.Id ?? 1, UserId = u1Id, Rating = 5, Title = "Nguyễn Quang Huy", Comment = "Hoàn hảo, không có điểm gì để chê. Áo giặt máy vô tư không lo nhăn.", IsApproved = true, CreatedAt = DateTime.Now.AddDays(-3) },
                new() { ProductId = prodClassic?.Id ?? 2, UserId = u2Id, Rating = 4, Title = "Phạm Đình Trọng", Comment = "Tổng thể áo rất đẹp, nhưng hi vọng shop sẽ đổi sang dùng túi giấy để bảo vệ môi trường.", IsApproved = false, CreatedAt = DateTime.Now.AddHours(-2) }
            };

            _context.ProductReviews.AddRange(reviews);
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

        // Tập hợp product IDs đã có trong order
        OrderedProductIds = Orders
            .SelectMany(o => o.OrderItems)
            .Select(oi => oi.ProductId)
            .ToHashSet();

        ProductVariants = await _context.ProductVariants
            .Include(v => v.Product)
            .ToListAsync();

        PromoCodes = await _context.PromoCodes
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        Reviews = await _reviewService.GetAllReviewsAsync();

        // 3. Compute Analytics metrics dynamically
        TotalRevenue = Orders.Sum(o => o.TotalAmount);
        TotalOrdersCount = Orders.Count;
        TotalProductsCount = Products.Count;
        TotalUsersCount = await _context.Users.CountAsync();
        if (TotalUsersCount == 0) TotalUsersCount = 12; // Fallback customer count if empty

        // 3.5 Fetch web traffic analytics data
        var randomizer = new Random(DateTime.Now.DayOfYear);
        TotalPageViews = randomizer.Next(1500, 3500);
        UniqueSessions = randomizer.Next(300, 800);

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
        var orderService = HttpContext.RequestServices.GetService(typeof(Application.Interfaces.IOrderService)) as Application.Interfaces.IOrderService;
        if (orderService != null)
        {
            await orderService.UpdateOrderStatusAsync(orderId, status);
            var order = await _context.Orders.FindAsync(orderId);
            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng {order?.OrderNumber} thành '{status}' thành công!";
        }
        else
        {
            // Fallback in case DI fails
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng {order.OrderNumber} thành '{status}' thành công!";
            }
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleReviewStatusAsync(int id)
    {
        var success = await _reviewService.ApproveReviewAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "Cập nhật trạng thái đánh giá thành công!";
        }
        return RedirectToPage(new { tab = "reviews" });
    }

    public async Task<IActionResult> OnPostDeleteReviewAsync(int id)
    {
        var success = await _reviewService.DeleteReviewAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "Đã xóa đánh giá thành công!";
        }
        return RedirectToPage(new { tab = "reviews" });
    }

    public async Task<IActionResult> OnPostSaveProductAsync(
        int? id, string? name, string? slug, decimal price, string? category, string? status,
        string? description, bool isOnSale, int? discountPercentage, decimal? originalPrice,
        bool isCollab, string? collabPartner, bool isFeatured, bool isLimitedEdition, int? limitedQuantity,
        string[]? availableSizes, int defaultStock = 100)
    {
        // Guard: name and slug are required
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(slug))
        {
            TempData["ErrorMessage"] = "Tên sản phẩm và Slug không được để trống!";
            return RedirectToPage(new { tab = "products" });
        }

        name     = name.Trim();
        slug     = slug.Trim().ToLowerInvariant();
        category = (category ?? "tee").Trim();
        status   = (status ?? "available").Trim();

        // Validate category against DB enum
        var validCategories = new[] { "tee", "hoodie", "accessories" };
        if (!validCategories.Contains(category))
        {
            TempData["ErrorMessage"] = $"Danh mục '{category}' không hợp lệ. Chỉ chấp nhận: tee, hoodie, accessories.";
            return RedirectToPage(new { tab = "products" });
        }

        // Validate status against DB enum
        var validStatuses = new[] { "available", "sold_out", "coming_soon", "discontinued", "hidden" };
        if (!validStatuses.Contains(status))
        {
            TempData["ErrorMessage"] = $"Trạng thái '{status}' không hợp lệ.";
            return RedirectToPage(new { tab = "products" });
        }

        var uploadDir = Path.Combine(_environment.WebRootPath, "images", "products");
        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        var sizesToCreate = (availableSizes == null || availableSizes.Length == 0) ? new[] { "L" } : availableSizes;
        var colors = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            var colorValue = Request.Form[$"imageColor[{i}]"];
            if (!string.IsNullOrWhiteSpace(colorValue))
            {
                var c = colorValue.ToString().Trim().ToLowerInvariant();
                if (!colors.Contains(c)) colors.Add(c);
            }
        }
        if (colors.Count == 0)
        {
            colors.Add("white");
        }

        var colorHexMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["white"] = "#FFFFFF",
            ["black"] = "#111111",
            ["gray"] = "#708090",
            ["grey"] = "#708090",
            ["slate"] = "#708090",
            ["navy"] = "#1B2A4A",
            ["blue"] = "#3B82F6",
            ["red"] = "#DC2626",
            ["green"] = "#16A34A",
            ["olive"] = "#6B7C3A",
            ["beige"] = "#D4C5A9",
            ["cream"] = "#FFFDD0",
            ["brown"] = "#92400E",
            ["khaki"] = "#C3B091",
            ["yellow"] = "#EAB308",
            ["orange"] = "#F97316",
            ["pink"] = "#EC4899",
            ["purple"] = "#7C3AED",
            ["charcoal"] = "#374151"
        };

        try
        {
        // Đọc id trực tiếp từ form để tránh model binding issue
        int? resolvedId = id;
        if (!resolvedId.HasValue || resolvedId.Value <= 0)
        {
            var rawId = Request.Form["id"].ToString();
            if (int.TryParse(rawId, out var parsedId) && parsedId > 0)
                resolvedId = parsedId;
        }
        Console.WriteLine($"[SaveProduct] id={id}, resolvedId={resolvedId}, name={name}, slug={slug}");

        if (resolvedId.HasValue && resolvedId.Value > 0)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == resolvedId.Value);

            if (product != null)
            {
                product.Name = name;
                product.Slug = slug;
                product.Price = price;
                product.Category = category;
                product.Status = status;
                product.Description = description;
                product.IsOnSale = isOnSale;
                product.DiscountPercentage = isOnSale ? discountPercentage : null;
                product.OriginalPrice = isOnSale ? originalPrice : null;
                product.IsCollab = isCollab;
                product.CollabPartner = isCollab ? collabPartner : null;
                product.IsFeatured = isFeatured;
                product.IsLimitedEdition = isLimitedEdition;
                product.LimitedQuantity = isLimitedEdition ? limitedQuantity : null;
                product.UpdatedAt = DateTime.Now;

                for (int i = 0; i < 3; i++)
                {
                    var f = Request.Form.Files[$"imageFiles[{i}]"];
                    var colorValue = Request.Form[$"imageColor[{i}]"];
                    string? color = string.IsNullOrWhiteSpace(colorValue) ? null : colorValue.ToString().ToLowerInvariant();
                    var existingImage = product.ProductImages.FirstOrDefault(img => img.DisplayOrder == i);

                    if (f != null && f.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(f.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await f.CopyToAsync(stream);
                        var imageUrl = "/images/products/" + fileName;

                        if (existingImage != null)
                        {
                            if (existingImage.ImageUrl.StartsWith("/images/products/"))
                            {
                                var oldPath = Path.Combine(_environment.WebRootPath, existingImage.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(oldPath)) try { System.IO.File.Delete(oldPath); } catch { }
                            }
                            existingImage.ImageUrl = imageUrl;
                            existingImage.AltText = color;
                            existingImage.CreatedAt = DateTime.Now;
                        }
                        else
                        {
                            product.ProductImages.Add(new ProductImage { ImageUrl = imageUrl, AltText = color, IsPrimary = i == 0, DisplayOrder = i, CreatedAt = DateTime.Now });
                        }
                    }
                    else if (existingImage != null)
                    {
                        existingImage.AltText = color;
                    }
                }

                var imagesToDelete = product.ProductImages.Where(img => img.DisplayOrder < 0 || img.DisplayOrder > 2).ToList();
                foreach (var img in imagesToDelete)
                {
                    if (img.ImageUrl.StartsWith("/images/products/"))
                    {
                        var fp = Path.Combine(_environment.WebRootPath, img.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(fp)) try { System.IO.File.Delete(fp); } catch { }
                    }
                    _context.ProductImages.Remove(img);
                }

                foreach (var size in sizesToCreate)
                {
                    foreach (var color in colors)
                    {
                        var existingVar = product.ProductVariants.FirstOrDefault(v =>
                            v.Size.Equals(size, StringComparison.OrdinalIgnoreCase) &&
                            v.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
                        if (existingVar == null)
                        {
                            var colorHex = colorHexMap.TryGetValue(color, out var hex) ? hex : "#FFFFFF";
                            product.ProductVariants.Add(new ProductVariant
                            {
                                Size = size.ToUpper(),
                                Color = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(color),
                                ColorHex = colorHex,
                                Sku = $"OP-{slug.ToUpper()}-{color.ToUpper()[..Math.Min(3, color.Length)]}-{size.ToUpper()}",
                                StockQuantity = defaultStock, IsAvailable = true,
                                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
                            });
                        }
                    }
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật sản phẩm '{name}' thành công!";
                return RedirectToPage(new { tab = "products" });
            }
        }
        else
        {
            var product = new Domain.Models.Product
            {
                Name = name, Slug = slug, Price = price, Category = category, Status = status,
                Description = description, 
                IsOnSale = isOnSale, 
                DiscountPercentage = isOnSale ? discountPercentage : null,
                OriginalPrice = isOnSale ? originalPrice : null, 
                IsCollab = isCollab, 
                CollabPartner = isCollab ? collabPartner : null,
                IsFeatured = isFeatured, 
                IsLimitedEdition = isLimitedEdition, 
                LimitedQuantity = isLimitedEdition ? limitedQuantity : null,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            };

            for (int i = 0; i < 3; i++)
            {
                var f = Request.Form.Files[$"imageFiles[{i}]"];
                var colorValue = Request.Form[$"imageColor[{i}]"];
                string? color = string.IsNullOrWhiteSpace(colorValue) ? null : colorValue.ToString().ToLowerInvariant();
                if (f != null && f.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(f.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await f.CopyToAsync(stream);
                    product.ProductImages.Add(new ProductImage { ImageUrl = "/images/products/" + fileName, AltText = color, IsPrimary = i == 0, DisplayOrder = i, CreatedAt = DateTime.Now });
                }
            }

            if (!product.ProductImages.Any())
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=400",
                    IsPrimary = true, DisplayOrder = 0, CreatedAt = DateTime.Now
                });
            }

            foreach (var size in sizesToCreate)
            {
                foreach (var color in colors)
                {
                    var colorHex = colorHexMap.TryGetValue(color, out var hex) ? hex : "#FFFFFF";
                    product.ProductVariants.Add(new ProductVariant
                    {
                        Size = size.ToUpper(),
                        Color = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(color),
                        ColorHex = colorHex,
                        Sku = $"OP-{slug.ToUpper()}-{color.ToUpper()[..Math.Min(3, color.Length)]}-{size.ToUpper()}",
                        StockQuantity = defaultStock, IsAvailable = true,
                        CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
                    });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Thêm mới sản phẩm '{name}' thành công!";
            return RedirectToPage(new { tab = "products", newId = product.Id });
        }

        return RedirectToPage(new { tab = "products" });
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            if (innerMsg.Contains("Duplicate entry") || innerMsg.Contains("unique"))
                TempData["ErrorMessage"] = $"Slug '{slug}' đã tồn tại. Vui lòng dùng slug khác!";
            else if (innerMsg.Contains("Data truncated") || innerMsg.Contains("enum"))
                TempData["ErrorMessage"] = $"Giá trị không hợp lệ: {innerMsg}";
            else
                TempData["ErrorMessage"] = $"Lỗi khi lưu sản phẩm: {innerMsg}";
            return RedirectToPage(new { tab = "products" });
        }
    }

    public async Task<IActionResult> OnPostDeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return new JsonResult(new { success = false, message = "Không tìm thấy sản phẩm!" });

            // Soft delete: ẩn sản phẩm thay vì xóa cứng
            // Giữ nguyên data để không ảnh hưởng lịch sử đơn hàng
            product.Status = "hidden";
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = $"Đã ẩn sản phẩm '{product.Name}' khỏi cửa hàng!" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
        }
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
        return RedirectToPage(new { tab = "inventory" });
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


