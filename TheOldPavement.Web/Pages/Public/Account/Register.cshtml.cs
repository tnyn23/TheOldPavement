using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Application.DTOs;
using TheOldPavement.Application.Exceptions;
using TheOldPavement.Application.Interfaces;

namespace TheOldPavement.Web.Pages.Public.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public RegisterModel(IAuthService authService)
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
            var dto = new CreateUserDTO
            {
                FullName = FullName,
                Email = Email,
                Password = Password
            };

            await _authService.RegisterAsync(dto);
            SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
            
            // Clear form
            ModelState.Clear();
            FullName = "";
            Email = "";
            
            return Page();
        }
        catch (BusinessException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "Đã có lỗi xảy ra. Vui lòng thử lại sau.";
            return Page();
        }
    }
}
