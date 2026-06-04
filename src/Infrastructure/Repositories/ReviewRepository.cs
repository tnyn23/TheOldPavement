using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository : Repository<ProductReview>, IReviewRepository
{
    public ReviewRepository(TheOldPavementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId, bool onlyApproved = true)
    {
        var query = _context.ProductReviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId);
            
        if (onlyApproved)
        {
            query = query.Where(r => r.IsApproved == true);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<ProductReview>> GetPendingReviewsAsync()
    {
        return await _context.ProductReviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.IsApproved != true)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
