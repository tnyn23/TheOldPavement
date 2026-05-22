using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Application.DTOs;
using TheOldPavement.Web.Helpers;

namespace TheOldPavement.Web.Pages;

public class WishlistModel : PageModel
{
    public List<WishlistItemDTO> WishlistItems { get; set; } = new();

    public void OnGet()
    {
        WishlistItems = WishlistManager.GetWishlist(HttpContext.Session);
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
