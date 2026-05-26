using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Customer;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    [BindProperty]
    public UpdateUserDTO UserProfile { get; set; } = new();

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public ChangePasswordViewModel PasswordModel { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public ProfileModel(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out int userId))
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                Email = user.Email;
                UserProfile.FullName = user.FullName;
                UserProfile.PhoneNumber = user.PhoneNumber;
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out int userId))
        {
            try
            {
                await _userService.UpdateUserAsync(userId, UserProfile);
                SuccessMessage = "Cập nhật thông tin thành công.";
                
                // Reload email
                var user = await _userService.GetUserByIdAsync(userId);
                if (user != null) Email = user.Email;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi cập nhật: " + ex.Message;
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        // Reload basic info
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out int userId))
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                Email = user.Email;
                UserProfile.FullName = user.FullName;
                UserProfile.PhoneNumber = user.PhoneNumber;
            }
            
            if (PasswordModel.NewPassword != PasswordModel.ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return Page();
            }

            try
            {
                await _authService.ChangePasswordAsync(userId, PasswordModel.OldPassword, PasswordModel.NewPassword);
                SuccessMessage = "Đổi mật khẩu thành công.";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        return Page();
    }
}

public class ChangePasswordViewModel
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
