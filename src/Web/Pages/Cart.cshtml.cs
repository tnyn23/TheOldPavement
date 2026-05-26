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

    // ── Standard form handlers (Cart page) ──────────────────────────
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

    // ── JSON handlers for the Cart Drawer (mini cart) ────────────────

    // GET /Cart?handler=SummaryJson
    public IActionResult OnGetSummaryJson()
    {
        return new JsonResult(new
        {
            totalItems = CartManager.GetTotalItems(HttpContext.Session),
            cartTotal  = CartManager.GetTotalPrice(HttpContext.Session)
        });
    }

    // POST /Cart?handler=UpdateQuantityJson  body: {itemId, quantity}
    public async Task<IActionResult> OnPostUpdateQuantityJsonAsync([FromBody] DrawerQtyRequest req)
    {
        if (req.Quantity <= 0)
        {
            CartManager.RemoveFromCart(HttpContext.Session, req.ItemId);
            return new JsonResult(new { removed = true,
                totalItems = CartManager.GetTotalItems(HttpContext.Session),
                cartTotal  = CartManager.GetTotalPrice(HttpContext.Session) });
        }

        CartManager.UpdateQuantity(HttpContext.Session, req.ItemId, req.Quantity);
        var cart     = CartManager.GetCart(HttpContext.Session);
        var lineItem = cart.FirstOrDefault(i => i.Id == req.ItemId);

        return new JsonResult(new
        {
            removed   = false,
            lineTotal  = lineItem?.TotalPrice ?? 0,
            cartTotal  = CartManager.GetTotalPrice(HttpContext.Session),
            totalItems = CartManager.GetTotalItems(HttpContext.Session)
        });
    }

    // POST /Cart?handler=RemoveJson  body: {itemId}
    public async Task<IActionResult> OnPostRemoveJsonAsync([FromBody] DrawerRemoveRequest req)
    {
        CartManager.RemoveFromCart(HttpContext.Session, req.ItemId);
        return new JsonResult(new
        {
            totalItems = CartManager.GetTotalItems(HttpContext.Session),
            cartTotal  = CartManager.GetTotalPrice(HttpContext.Session)
        });
    }

    // ── Private helpers ──────────────────────────────────────────────
    private void LoadCart()
    {
        CartItems  = CartManager.GetCart(HttpContext.Session);
        TotalPrice = CartManager.GetTotalPrice(HttpContext.Session);
        TotalItems = CartManager.GetTotalItems(HttpContext.Session);
    }
}

// Request DTOs for JSON handlers
public record DrawerQtyRequest(int ItemId, int Quantity);
public record DrawerRemoveRequest(int ItemId);
