using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.DTOs;
using Web.Helpers;

namespace Web.Pages;

public class CartModel : PageModel
{
    public List<CartItemDTO> CartItems { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }

    public void OnGet()
    {
        LoadCart();
    }

    public IActionResult OnPostUpdateQuantity(int itemId, int quantity)
    {
        CartManager.UpdateQuantity(HttpContext.Session, itemId, quantity);
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int itemId)
    {
        CartManager.RemoveFromCart(HttpContext.Session, itemId);
        return RedirectToPage();
    }

    private void LoadCart()
    {
        CartItems = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);
        TotalItems = CartManager.GetTotalItems(HttpContext.Session);
    }
}

