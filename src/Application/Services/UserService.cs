using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDTO?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    public Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateUserAsync(CreateUserDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateUserAsync(int id, UpdateUserDTO dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
        {
            user.FullName = dto.FullName;
            user.Phone = dto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }
    }

    public Task DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
    }
}
