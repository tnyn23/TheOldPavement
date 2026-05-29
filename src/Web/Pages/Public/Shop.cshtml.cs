using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;

namespace Web.Pages.Public;

public class ShopModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public ShopModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public List<Domain.Models.Product> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public const int PageSize = 12;

    [BindProperty(Name = "Page", SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "newest";

    public async Task OnGetAsync()
    {
        if (Request.Query.TryGetValue("Page", out var pVal) && int.TryParse(pVal, out var parsedPage))
        {
            PageNumber = parsedPage;
        }
        else
        {
            PageNumber = 1;
        }

        if (PageNumber < 1) PageNumber = 1;

        var query = _context.Products
            .Include(p => p.ProductImages)
            .Where(p => p.Status != "hidden");

        // 1. Search Filter
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var searchLower = SearchQuery.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.Slug.Contains(searchLower));
        }

        // 2. Category Filter
        if (!string.IsNullOrWhiteSpace(Category) && Category != "all")
        {
            query = query.Where(p => p.Category == Category);
        }

        // 3. Price Filter
        if (MinPrice.HasValue)
            query = query.Where(p => p.Price >= MinPrice.Value);
        if (MaxPrice.HasValue)
            query = query.Where(p => p.Price <= MaxPrice.Value);

        // 4. Sorting
        query = SortBy switch
        {
            "price_asc"   => query.OrderBy(p => p.Price),
            "price_desc"  => query.OrderByDescending(p => p.Price),
            "name_asc"    => query.OrderBy(p => p.Name),
            "name_desc"   => query.OrderByDescending(p => p.Name),
            "bestselling" => query.OrderByDescending(p => p.OrderItems.Count),
            _             => query.OrderByDescending(p => p.CreatedAt)
        };

        // 5. Pagination
        TotalCount = await query.CountAsync();
        TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
        if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

        Products = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}


