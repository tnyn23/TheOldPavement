using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Context;

namespace Web.Pages.Public.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly TheOldPavementDbContext _context;
    private readonly IEmailService _emailService;

    [BindProperty]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public ForgotPasswordModel(TheOldPavementDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email.ToLower().Trim());
            if (user == null)
            {
                // To prevent email enumeration, we can either say "We've sent a link if account exists"
                // or just say "Email không tồn tại trong hệ thống" for user friendliness in student projects.
                // Let's go with "Email không tồn tại trong hệ thống" for clarity.
                ErrorMessage = "Email này không tồn tại trong hệ thống.";
                return Page();
            }

            // Generate a random temporary password
            var rawPassword = "TOP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
            
            // Hash password using SHA256 (same as AuthService)
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawPassword));
            var hash = Convert.ToBase64String(bytes);

            // Update user password hash
            user.PasswordHash = hash;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send password recovery email
            await _emailService.SendPasswordRecoveryEmailAsync(user.Email, user.FullName ?? "Khách hàng", rawPassword);

            SuccessMessage = "Mật khẩu mới đã được gửi vào email của bạn. Vui lòng kiểm tra hộp thư đến (hoặc hộp thư rác).";
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


