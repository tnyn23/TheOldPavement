using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Web.Helpers;

namespace Web.Pages;

public class WishlistModel : PageModel
{
    private readonly Infrastructure.Context.TheOldPavementDbContext _context;

    public List<WishlistItemDTO> WishlistItems { get; set; } = new();
    public List<Domain.Models.Product> RecentlyViewedProducts { get; set; } = new();
    public List<Domain.Models.Product> RecommendedProducts { get; set; } = new();

    public WishlistModel(Infrastructure.Context.TheOldPavementDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        WishlistItems = WishlistManager.GetWishlist(HttpContext.Session);

        // Fetch recently viewed products from database
        var recentlyViewedIds = RecentlyViewedManager.GetRecentlyViewed(HttpContext.Session);
        if (recentlyViewedIds.Any())
        {
            var productsMap = await _context.Products
                .Where(p => recentlyViewedIds.Contains(p.Id))
                .Include(p => p.ProductImages)
                .ToDictionaryAsync(p => p.Id);

            // Maintain order of insertion
            foreach (var id in recentlyViewedIds)
            {
                if (productsMap.TryGetValue(id, out var product))
                {
                    RecentlyViewedProducts.Add(product);
                }
            }
        }

        // Fetch recommended products (e.g. 4 random products, excluding wishlist items)
        var wishlistProductIds = WishlistItems.Select(w => w.ProductId).ToList();
        RecommendedProducts = await _context.Products
            .Where(p => !wishlistProductIds.Contains(p.Id))
            .OrderBy(p => Guid.NewGuid())
            .Take(4)
            .Include(p => p.ProductImages)
            .ToListAsync();
    }

    public IActionResult OnPostRemove(int productId)
    {
        var wishlist = WishlistManager.GetWishlist(HttpContext.Session);
        var existing = wishlist.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            wishlist.Remove(existing);
            WishlistManager.SaveWishlist(HttpContext.Session, wishlist);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostAddToCart(int productId, string name, decimal price, string slug, string imageUrl)
    {
        // Add item to cart
        var cartItem = new CartItemDTO
        {
            ProductId = productId,
            VariantId = 1,
            ProductName = name,
            UnitPrice = price,
            Quantity = 1,
            Size = "L", // default
            Color = "white", // default
            ProductThumbnail = imageUrl
        };
        CartManager.AddToCart(HttpContext.Session, cartItem);

        // Remove from wishlist
        var wishlist = WishlistManager.GetWishlist(HttpContext.Session);
        var existing = wishlist.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            wishlist.Remove(existing);
            WishlistManager.SaveWishlist(HttpContext.Session, wishlist);
        }

        TempData["SuccessMessage"] = $"Đã thêm {name} vào giỏ hàng thành công!";
        return RedirectToPage();
    }
}

