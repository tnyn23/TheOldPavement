using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Helpers;

namespace Web.Pages;

public class ThankYouCardModel : PageModel
{
    public string? OrderNumber { get; set; }

    public void OnGet(string? orderNumber)
    {
        if (TempData.ContainsKey("OrderedNumber"))
        {
            OrderNumber = TempData["OrderedNumber"] as string;
        }
        else if (!string.IsNullOrEmpty(orderNumber))
        {
            OrderNumber = orderNumber;
        }

        // Clear cart session when order is completed
        CartManager.ClearCart(HttpContext.Session);
    }
}

