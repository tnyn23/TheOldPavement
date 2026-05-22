using Microsoft.EntityFrameworkCore;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;
using TheOldPavement.Infrastructure.Context;

namespace TheOldPavement.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(TheOldPavementDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}

