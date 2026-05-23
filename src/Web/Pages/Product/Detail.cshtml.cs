using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs;
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

    public List<ColorOption> Colors { get; set; } = new()
    {
        new() { Name = "White", Value = "white", Hex = "#FFFFFF" },
        new() { Name = "Black", Value = "black", Hex = "#000000" },
        new() { Name = "Slate Gray", Value = "gray", Hex = "#708090" }
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

    public DetailModel(IProductRepository productRepository, TheOldPavementDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
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

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return RedirectToPage("/Index");
        }

        // Try getting from DB (include images/variants)
        Product = await _productRepository.GetBySlugAsync(slug);

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

                // Set initial color to white if available, otherwise first group
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
        }

        // Fetch Related Products (4 items from the same category)
        string category = Product?.Category ?? "Tee";
        RelatedProducts = await _context.Products
            .Where(p => p.Category == category && p.Slug != slug)
            .OrderBy(r => Guid.NewGuid()) // Random order
            .Take(4)
            .Include(p => p.ProductImages)
            .ToListAsync();

        // Fetch Product Reviews
        if (Product != null)
        {
            Reviews = await _context.ProductReviews
                .Where(r => r.ProductId == Product.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(string slug)
    {
        Product = await _productRepository.FirstOrDefaultAsync(p => p.Slug == slug);

        if (Product == null)
        {
            GenerateFallbackProduct(slug);
        }

        var cartItem = new CartItemDTO
        {
            ProductId = Product?.Id ?? 0,
            VariantId = 1, // Fallback variant ID
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
        Product = await _productRepository.FirstOrDefaultAsync(p => p.Slug == slug);

        if (Product == null)
        {
            GenerateFallbackProduct(slug);
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
            // Get current user ID if logged in, otherwise use 0 for guest
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            int userId = 0;
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
            {
                userId = parsedId;
            }

            var review = new ProductReview
            {
                ProductId = Product.Id,
                UserId = userId,
                Rating = Rating,
                Title = string.IsNullOrWhiteSpace(ReviewerName) ? "Khách hàng" : ReviewerName, // Reuse Title field for display name
                Comment = ReviewContent,
                CreatedAt = DateTime.Now
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Cảm ơn bạn! Đánh giá của bạn đã được gửi thành công.";
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


