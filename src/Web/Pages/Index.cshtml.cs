using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;

namespace Web.Pages;

public class IndexModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public IndexModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public List<Domain.Models.Product> FeaturedProducts { get; set; } = new();
    public List<Domain.Models.Product> NewArrivals { get; set; } = new();
    public List<Domain.Models.Product> SaleProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Featured products (IsFeatured = true, or fallback to newest 4)
        FeaturedProducts = await _context.Products
            .Include(p => p.ProductImages)
            .Where(p => p.Status != "hidden" && p.IsFeatured == true)
            .OrderByDescending(p => p.CreatedAt)
            .Take(4)
            .ToListAsync();

        if (!FeaturedProducts.Any())
        {
            FeaturedProducts = await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.Status != "hidden")
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();
        }

        // New arrivals — latest 8
        NewArrivals = await _context.Products
            .Include(p => p.ProductImages)
            .Where(p => p.Status != "hidden")
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        // Sale products
        SaleProducts = await _context.Products
            .Include(p => p.ProductImages)
            .Where(p => p.Status != "hidden" && p.IsOnSale == true)
            .OrderByDescending(p => p.DiscountPercentage)
            .Take(4)
            .ToListAsync();
    }
}
