using Microsoft.EntityFrameworkCore;
using TheOldPavement.Core.Interfaces;
using TheOldPavement.Core.Models;
using TheOldPavement.Data.Context;

namespace TheOldPavement.Data.Repositories;

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
