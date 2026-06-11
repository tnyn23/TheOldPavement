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

    [BindProperty(Name = "pg", SupportsGet = true)]
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

    private static readonly Dictionary<string, List<string>> Synonyms = new(StringComparer.OrdinalIgnoreCase)
    {
        { "áo thun", new List<string> { "tee", "tshirt", "t-shirt" } },
        { "tee", new List<string> { "áo thun", "tshirt", "t-shirt" } },
        { "hoodie", new List<string> { "áo khoác nỉ", "áo nỉ", "sweater", "khoác nỉ" } },
        { "áo khoác nỉ", new List<string> { "hoodie", "áo nỉ", "sweater" } },
        { "áo khoác", new List<string> { "jacket", "coat" } },
        { "jacket", new List<string> { "áo khoác", "coat" } },
        { "quần", new List<string> { "pants", "trousers" } },
        { "pants", new List<string> { "quần", "trousers" } },
        { "phụ kiện", new List<string> { "accessories", "bag", "hat", "túi", "nón", "mũ" } }
    };

    public async Task OnGetAsync()
    {
        if (PageNumber < 1) PageNumber = 1;

        var query = _context.Products
            .Include(p => p.ProductImages)
            .Where(p => p.Status != "hidden");

        // 1. Search Filter
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var searchLower = SearchQuery.ToLower().Trim();
            var searchTerms = new List<string> { searchLower };
            
            foreach (var kvp in Synonyms)
            {
                if (searchLower.Contains(kvp.Key.ToLower()))
                {
                    searchTerms.AddRange(kvp.Value);
                }
            }

            var productsIds = await _context.Products.Select(p => new { p.Id, p.Name, p.Slug }).ToListAsync();
            var matchedIds = productsIds
                .Where(p => searchTerms.Any(term => p.Name.ToLower().Contains(term) || p.Slug.ToLower().Contains(term)))
                .Select(p => p.Id).ToList();

            query = query.Where(p => matchedIds.Contains(p.Id));
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

    public async Task<IActionResult> OnGetSearchSuggestionsAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return new JsonResult(new List<object>());

        var searchLower = q.ToLower().Trim();
        var searchTerms = new List<string> { searchLower };
        
        foreach (var kvp in Synonyms)
        {
            if (searchLower.Contains(kvp.Key.ToLower()))
            {
                searchTerms.AddRange(kvp.Value);
            }
        }

        var productsIds = await _context.Products.Where(p => p.Status != "hidden").Select(p => new { p.Id, p.Name, p.Slug }).ToListAsync();
        var matchedIds = productsIds
            .Where(p => searchTerms.Any(term => p.Name.ToLower().Contains(term) || p.Slug.ToLower().Contains(term)))
            .Select(p => p.Id).ToList();

        var suggestions = await _context.Products
            .Include(p => p.ProductImages)
            .Where(p => matchedIds.Contains(p.Id))
            .Take(6)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                slug = p.Slug,
                price = p.Price,
                image = p.ProductImages.FirstOrDefault(i => i.IsPrimary == true) != null 
                    ? p.ProductImages.FirstOrDefault(i => i.IsPrimary == true)!.ImageUrl 
                    : (p.ProductImages.FirstOrDefault() != null ? p.ProductImages.FirstOrDefault()!.ImageUrl : "")
            })
            .ToListAsync();

        return new JsonResult(suggestions);
    }
}


