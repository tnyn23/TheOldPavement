using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using TheOldPavement.Application.DTOs;
using TheOldPavement.Application.Exceptions;
using TheOldPavement.Application.Interfaces;
using TheOldPavement.Domain.Constants;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;

namespace TheOldPavement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
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

    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
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

