using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Interfaces;

namespace Web.Pages.Public.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly IAuthService _authService;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public ForgotPasswordModel(IAuthService authService)
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

        try
        {
            // Call the application service to handle the forgot password business logic
            // To prevent email enumeration attacks, this method always returns true 
            // even if the email doesn't exist.
            await _authService.ForgotPasswordAsync(Email.ToLower().Trim());

            SuccessMessage = "Nếu email của bạn tồn tại trong hệ thống, một mật khẩu mới đã được gửi vào hộp thư. Vui lòng kiểm tra (kể cả hộp thư rác).";
            Email = "";
            ModelState.Clear();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý: " + ex.Message;
        }

        return Page();
    }
}
