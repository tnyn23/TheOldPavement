using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Constants;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IMapper mapper, IEmailService emailService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<AuthResultDTO?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null; // Invalid credentials
        }

        var userDto = _mapper.Map<UserDTO>(user);
        
        return new AuthResultDTO
        {
            User = userDto,
            Token = "COOKIE_AUTH", // Placeholder, since we use cookies
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }

    public async Task<bool> RegisterAsync(CreateUserDTO dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new BusinessException(string.Format(ErrorMessages.AlreadyExists, "Email"));
        }

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            Phone = dto.PhoneNumber,
            Role = "customer",
            PasswordHash = HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new BusinessException("Không tìm thấy người dùng.");
        }

        if (!VerifyPassword(oldPassword, user.PasswordHash))
        {
            throw new BusinessException("Mật khẩu cũ không chính xác.");
        }

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            // Always return true to prevent email enumeration attacks
            return true; 
        }

        // Generate a random temporary password
        var rawPassword = "TOP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
        
        user.PasswordHash = HashPassword(rawPassword);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        // Send email với timeout 12s — await thẳng để Railway log được lỗi
        try
        {
            var emailTask = _emailService.SendPasswordRecoveryEmailAsync(user.Email, user.FullName ?? "Khách hàng", rawPassword);
            var timeoutTask = Task.Delay(12000);
            await Task.WhenAny(emailTask, timeoutTask);
            if (emailTask.IsCompletedSuccessfully)
                Console.WriteLine($"[Email] Sent successfully to {user.Email}");
            else
                Console.WriteLine($"[Email] Timeout or pending for {user.Email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email ERROR] {ex.GetType().Name}: {ex.Message}");
        }
        
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            throw new BusinessException("Email không hợp lệ.");
        }

        // Validate token (using dummy logic matching ForgotPassword)
        if (token != "123456")
        {
            throw new BusinessException("Mã xác nhận không chính xác hoặc đã hết hạn.");
        }

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public string GenerateToken(UserDTO user)
    {
        return "COOKIE_AUTH";
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}


