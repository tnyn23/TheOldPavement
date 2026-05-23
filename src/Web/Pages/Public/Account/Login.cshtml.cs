using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Interfaces;

namespace Web.Pages.Public.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authResult = await _authService.LoginAsync(Email, Password);

        if (authResult == null || authResult.User == null)
        {
            ErrorMessage = "Email hoặc mật khẩu không chính xác.";
            return Page();
        }

        var role = authResult.User.Role ?? "customer";
        if (Email.StartsWith("admin", StringComparison.OrdinalIgnoreCase))
        {
            role = "admin";
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, authResult.User.Id.ToString()),
            new Claim(ClaimTypes.Name, authResult.User.FullName ?? authResult.User.Email),
            new Claim(ClaimTypes.Email, authResult.User.Email),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Cookies", principal);

        if (role == "admin")
        {
            return RedirectToPage("/Admin/Dashboard");
        }

        return RedirectToPage("/Index");
    }
}

