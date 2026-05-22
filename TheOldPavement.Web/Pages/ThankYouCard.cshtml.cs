using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheOldPavement.Web.Pages;

public class ThankYouCardModel : PageModel
{
    public string? OrderNumber { get; set; }

    public void OnGet()
    {
        if (TempData.ContainsKey("OrderedNumber"))
        {
            OrderNumber = TempData["OrderedNumber"] as string;
        }
    }
}
