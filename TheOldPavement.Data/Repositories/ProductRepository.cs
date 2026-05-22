using Microsoft.EntityFrameworkCore;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;
using TheOldPavement.Infrastructure.Context;

namespace TheOldPavement.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(TheOldPavementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Where(p => p.Status == "available")
            .OrderByDescending(p => p.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
    {
        return await _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Where(p => p.Name.Contains(keyword) || (p.Description != null && p.Description.Contains(keyword)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }
}

