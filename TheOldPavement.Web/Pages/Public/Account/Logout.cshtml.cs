using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheOldPavement.Web.Pages.Public.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        HttpContext.Session.Clear(); // Clear session too!
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        HttpContext.Session.Clear(); // Clear session too!
        return RedirectToPage("/Index");
    }
}
