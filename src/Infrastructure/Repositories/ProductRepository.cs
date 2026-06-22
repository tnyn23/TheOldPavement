using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

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

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}


