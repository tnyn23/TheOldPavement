using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Web.Helpers;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Web.Pages.Product;

public class DetailModel : PageModel
{
    private readonly IProductRepository _productRepository;
    private readonly TheOldPavementDbContext _context;

    public Domain.Models.Product? Product { get; set; }
    public List<string> Sizes { get; set; } = new() { "S", "M", "L", "XL", "XXL" };
    public List<string> ProductImagesList { get; set; } = new();
    public Dictionary<string, List<string>> ProductImagesByColor { get; set; } = new();
    
    public List<Domain.Models.Product> RelatedProducts { get; set; } = new();
    public List<ProductReview> Reviews { get; set; } = new();
    
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> RatingCounts { get; set; } = new() { {5, 0}, {4, 0}, {3, 0}, {2, 0}, {1, 0} };
    public Dictionary<int, double> RatingPercentages { get; set; } = new() { {5, 0}, {4, 0}, {3, 0}, {2, 0}, {1, 0} };
    public int? SelectedRatingFilter { get; set; }

    public List<ColorOption> Colors { get; set; } = new();

    // Maps common color names (lowercase) → CSS hex values
    private static readonly Dictionary<string, string> ColorHexMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["white"]       = "#FFFFFF",
        ["black"]       = "#111111",
        ["gray"]        = "#708090",
        ["grey"]        = "#708090",
        ["slate"]       = "#708090",
        ["slate gray"]  = "#708090",
        ["slate grey"]  = "#708090",
        ["navy"]        = "#1B2A4A",
        ["blue"]        = "#3B82F6",
        ["red"]         = "#DC2626",
        ["green"]       = "#16A34A",
        ["olive"]       = "#6B7C3A",
        ["beige"]       = "#D4C5A9",
        ["cream"]       = "#FFFDD0",
        ["brown"]       = "#92400E",
        ["khaki"]       = "#C3B091",
        ["yellow"]      = "#EAB308",
        ["orange"]      = "#F97316",
        ["pink"]        = "#EC4899",
        ["purple"]      = "#7C3AED",
        ["charcoal"]    = "#374151",
        ["off white"]   = "#F5F5F0",
        ["off-white"]   = "#F5F5F0",
        ["light gray"]  = "#D1D5DB",
        ["light grey"]  = "#D1D5DB",
        ["dark gray"]   = "#374151",
        ["dark grey"]   = "#374151",
        ["sand"]        = "#C2B280",
        ["stone"]       = "#A8A29E",
        ["ecru"]        = "#C2B280",
        ["natural"]     = "#E8DCC8",
        ["vintage"]     = "#C4A882",
    };

    [BindProperty]
    public string SelectedSize { get; set; } = "L";

    [BindProperty]
    public string SelectedColor { get; set; } = "white";

    [BindProperty]
    public int Quantity { get; set; } = 1;

    [BindProperty]
    public int Rating { get; set; } = 5;

    [BindProperty]
    public string ReviewContent { get; set; } = string.Empty;

    [BindProperty]
    public string ReviewerName { get; set; } = string.Empty;

    private readonly IReviewService _reviewService;
    private readonly IWebHostEnvironment _env;

    [BindProperty]
    public List<IFormFile>? ReviewImages { get; set; }

    public DetailModel(IProductRepository productRepository, TheOldPavementDbContext context, IReviewService reviewService, IWebHostEnvironment env)
    {
        _productRepository = productRepository;
        _context = context;
        _reviewService = reviewService;
        _env = env;
    }

    private void GenerateFallbackProduct(string slug)
    {
        // Default text helpers
        string name = System.Text.RegularExpressions.Regex.Replace(slug, @"[-_]+", " ").ToUpper();
        decimal price = 425000;
        string description = $"Sản phẩm đặc biệt thuộc bộ sưu tập The Old Pavement. Được sản xuất từ chất liệu Cotton 100% cao cấp dày dặn (220 GSM), form dáng oversized thời thượng rộng rãi thoải mái cùng đường may hai kim cực kỳ tinh tế.";

        if (slug.Contains("rolling-stones") || slug.Contains("rs-"))
        {
            name = slug.Contains("limited") ? "THE ROLLING STONES × OP LIMITED TEE" : "THE ROLLING STONES × OP VINTAGE TEE";
            price = 550000;
            description = "Sản phẩm nằm trong bộ sưu tập chính thức (Official Collaboration) giữa The Old Pavement và ban nhạc Rock huyền thoại The Rolling Stones. Nổi bật với họa tiết Tongue Logo huyền thoại lồng ghép cùng các nét vẽ xích lô Hà Nội độc đáo. Đi kèm thẻ chứng nhận số thứ tự phiên bản giới hạn.";
        }
        else if (slug.Contains("classic-black"))
        {
            name = "THE OLD PAVEMENT CLASSIC BLACK TEE";
            price = 425000;
            description = "Chiếc áo thun đen kinh điển không thể thiếu trong tủ đồ của bất kỳ tín đồ thời trang đường phố nào. Form dáng boxy oversized cực kỳ cứng cáp và tôn dáng.";
        }
        else if (slug.Contains("commercial") || slug.Contains("cp-"))
        {
            name = "THE OLD PAVEMENT COMMERCIAL UTILITY JACKET";
            price = 899000;
            description = "Thiết kế Jacket đa túi mang phong cách Tactical/Utility cao cấp, cản gió nhẹ và chống thấm nước tốt, hoàn thiện với tem nhãn thêu tay độc quyền.";
        }

        Product = new Domain.Models.Product
        {
            Id = 999, // Fallback ID
            Name = name,
            Price = price,
            Slug = slug,
            Description = description
        };

        // Populate Images based on slug
        if (slug.Contains("rolling-stones") || slug.Contains("rs-"))
        {
            ProductImagesList.Add("https://images.unsplash.com/photo-1731267776886-90f90af75eb1?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1627225793904-a2f900a6e4cf?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
        }
        else if (slug.Contains("black") || slug.Contains("dark"))
        {
            ProductImagesList.Add("https://images.unsplash.com/photo-1662103627854-ae7551d1eddb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
        }
        else
        {
            ProductImagesList.Add("https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1662103627854-ae7551d1eddb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
            ProductImagesList.Add("https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
        }
    }

    public async Task<IActionResult> OnGetAsync(string slug, int? ratingFilter = null)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return RedirectToPage("/Index");
        }

        SelectedRatingFilter = ratingFilter;

        // Try getting from DB (include images/variants)
        Product = await _productRepository.GetBySlugAsync(slug);

        if (Product != null)
        {
            RecentlyViewedManager.AddRecentlyViewed(HttpContext.Session, Product.Id);
        }

        if (Product == null)
        {
            // Instead of 404, we dynamically generate a high-quality fallback product page!
            GenerateFallbackProduct(slug);
        }
        else
        {
            // Populate image from database if any, otherwise set defaults
            if (Product.ProductImages != null && Product.ProductImages.Any())
            {
                // Group images by color (stored in AltText)
                var groups = Product.ProductImages
                    .GroupBy(pi => (pi.AltText ?? "default").ToLowerInvariant());

                foreach (var g in groups)
                {
                    ProductImagesByColor[g.Key] = g.Select(x => x.ImageUrl).ToList();
                }

                // Set initial color to first group (prefer white if exists)
                var initial = ProductImagesByColor.ContainsKey("white") ? "white" : ProductImagesByColor.Keys.FirstOrDefault() ?? "default";
                SelectedColor = initial;
                ProductImagesList = ProductImagesByColor.ContainsKey(initial) ? ProductImagesByColor[initial] : new List<string>();
            }
            else
            {
                // Set default images
                if (Product.Slug.Contains("black"))
                {
                    ProductImagesList.Add("https://images.unsplash.com/photo-1662103627854-ae7551d1eddb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                    ProductImagesList.Add("https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                    ProductImagesList.Add("https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                }
                else
                {
                    ProductImagesList.Add("https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                    ProductImagesList.Add("https://images.unsplash.com/photo-1662103627854-ae7551d1eddb?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                    ProductImagesList.Add("https://images.unsplash.com/photo-1695131023163-1e04e1345a91?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080");
                }
            }

            // ── Build Colors list dynamically from image color keys ──────────────
            // Priority: variant ColorHex → ColorHexMap → fallback gray
            var variantHexLookup = Product.ProductVariants?
                .Where(v => !string.IsNullOrEmpty(v.Color) && !string.IsNullOrEmpty(v.ColorHex))
                .GroupBy(v => v.Color!.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.First().ColorHex!) 
                ?? new Dictionary<string, string>();

            var colorKeys = ProductImagesByColor.Keys.Where(k => k != "default").ToList();
            if (colorKeys.Any())
            {
                Colors = colorKeys.Select(key =>
                {
                    string hex = variantHexLookup.TryGetValue(key, out var vh) && !string.IsNullOrEmpty(vh)
                        ? vh
                        : ColorHexMap.TryGetValue(key, out var mh) ? mh : "#9CA3AF";
                    return new ColorOption
                    {
                        Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(key),
                        Value = key,
                        Hex = hex
                    };
                }).ToList();
            }
            else if (Product.ProductVariants != null && Product.ProductVariants.Any(v => !string.IsNullOrEmpty(v.Color)))
            {
                Colors = Product.ProductVariants
                    .Where(v => !string.IsNullOrEmpty(v.Color))
                    .GroupBy(v => v.Color!.ToLowerInvariant())
                    .Select(g =>
                    {
                        var first = g.First();
                        string hex = !string.IsNullOrEmpty(first.ColorHex)
                            ? first.ColorHex
                            : ColorHexMap.TryGetValue(g.Key, out var mh) ? mh : "#9CA3AF";
                        return new ColorOption
                        {
                            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
                            Value = g.Key,
                            Hex = hex
                        };
                    }).ToList();
            }
            // else Colors stays as empty list — fallback handled in Razor
        }

        // Fetch Related Products (4 items from the same category)
        string category = Product?.Category ?? "Tee";
        RelatedProducts = await _context.Products
            .Where(p => p.Category == category && p.Slug != slug)
            .OrderBy(r => Guid.NewGuid()) // Random order
            .Take(4)
            .Include(p => p.ProductImages)
            .ToListAsync();

        // Fetch Product Reviews using service
        if (Product != null)
        {
            var allReviews = await _reviewService.GetProductReviewsAsync(Product.Id);

            TotalReviews = allReviews.Count;
            if (TotalReviews > 0)
            {
                AverageRating = Math.Round(allReviews.Average(r => r.Rating), 1);
                for (int star = 1; star <= 5; star++)
                {
                    int count = allReviews.Count(r => r.Rating == star);
                    RatingCounts[star] = count;
                    RatingPercentages[star] = Math.Round((double)count / TotalReviews * 100, 1);
                }
            }
            else
            {
                AverageRating = 0;
            }

            if (ratingFilter.HasValue && ratingFilter.Value >= 1 && ratingFilter.Value <= 5)
            {
                Reviews = allReviews.Where(r => r.Rating == ratingFilter.Value).ToList();
            }
            else
            {
                Reviews = allReviews;
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(string slug)
    {
        Product = await _context.Products
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (Product == null)
        {
            GenerateFallbackProduct(slug);
        }

        // Find the variant matching selected size and color
        var matchingVariant = Product?.ProductVariants?
            .FirstOrDefault(v => v.Size.Equals(SelectedSize, StringComparison.OrdinalIgnoreCase) && 
                                 v.Color.Equals(SelectedColor, StringComparison.OrdinalIgnoreCase));
        
        int variantId = matchingVariant?.Id ?? (Product?.ProductVariants?.FirstOrDefault()?.Id ?? 1);

        var cartItem = new CartItemDTO
        {
            ProductId = Product?.Id ?? 0,
            VariantId = variantId,
            ProductName = Product?.Name ?? "Sản phẩm",
            UnitPrice = Product?.Price ?? 0,
            Quantity = Quantity,
            Size = SelectedSize,
            Color = SelectedColor,
            ProductThumbnail = ProductImagesList.Any() ? ProductImagesList[0] : "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx3aGl0ZSUyMGJsYW5rJTIwdC1zaGlydCUyMG1vY2t1cCUyMGZsYXQlMjBsYXl8ZW58MXx8fHwxNzczNjc0Mzg3fDA&ixlib=rb-4.1.0&q=80&w=1080"
        };

        CartManager.AddToCart(HttpContext.Session, cartItem);

        TempData["SuccessMessage"] = $"Đã thêm {Quantity} sản phẩm vào giỏ hàng thành công!";

        return RedirectToPage(new { slug });
    }

    public async Task<IActionResult> OnPostAddToWishlistAsync(string slug)
    {
        // Load product WITH images so we can store the real thumbnail
        Product = await _context.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (Product == null)
        {
            GenerateFallbackProduct(slug);
        }
        else if (Product.ProductImages != null && Product.ProductImages.Any())
        {
            ProductImagesList = Product.ProductImages
                .Select(pi => pi.ImageUrl)
                .ToList();
        }

        var wishlistItem = new WishlistItemDTO
        {
            ProductId = Product?.Id ?? 0,
            ProductName = Product?.Name ?? "Sản phẩm",
            Price = Product?.Price ?? 0,
            Slug = slug,
            ImageUrl = ProductImagesList.Any() ? ProductImagesList[0] : "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080"
        };

        bool wasInWishlist = WishlistManager.IsInWishlist(HttpContext.Session, wishlistItem.ProductId);
        WishlistManager.ToggleWishlist(HttpContext.Session, wishlistItem);

        if (wasInWishlist)
        {
            TempData["SuccessMessage"] = $"Đã xóa {Product?.Name} khỏi danh sách yêu thích.";
        }
        else
        {
            TempData["SuccessMessage"] = $"Đã thêm {Product?.Name} vào danh sách yêu thích thành công!";
        }

        return RedirectToPage(new { slug });
    }

    public async Task<IActionResult> OnPostAddReviewAsync(string slug)
    {
        Product = await _productRepository.FirstOrDefaultAsync(p => p.Slug == slug);
        if (Product == null) return RedirectToPage("/Index");

        if (!string.IsNullOrWhiteSpace(ReviewContent))
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            int userId = 0;
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
            {
                userId = parsedId;
            }

            if (userId == 0)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá sản phẩm.";
                return RedirectToPage(new { slug });
            }

            bool canReview = await _reviewService.CanUserReviewProductAsync(userId, Product.Id);
            if (!canReview)
            {
                TempData["ErrorMessage"] = "Chỉ những khách hàng đã mua và nhận sản phẩm này mới được đánh giá.";
                return RedirectToPage(new { slug });
            }

            var imageUrls = new List<string>();
            if (ReviewImages != null && ReviewImages.Any())
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "uploads", "reviews");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                foreach (var file in ReviewImages)
                {
                    if (file.Length > 0)
                    {
                        string ext = Path.GetExtension(file.FileName);
                        string newFileName = $"{Guid.NewGuid()}{ext}";
                        string filePath = Path.Combine(uploadDir, newFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        imageUrls.Add($"/uploads/reviews/{newFileName}");
                    }
                }
            }

            await _reviewService.AddReviewAsync(Product.Id, userId, Rating, ReviewContent, ReviewerName, imageUrls);
            
            TempData["SuccessMessage"] = "Cảm ơn bạn! Đánh giá của bạn đã được gửi thành công.";
        }

        return RedirectToPage(new { slug });
    }

    public async Task<IActionResult> OnPostMarkHelpfulAsync(string slug, int reviewId)
    {
        bool success = await _reviewService.MarkHelpfulAsync(reviewId);
        if (success)
        {
            TempData["SuccessMessage"] = "Đã đánh dấu đánh giá là hữu ích.";
        }
        return RedirectToPage(new { slug });
    }
}

public class ColorOption
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
}


