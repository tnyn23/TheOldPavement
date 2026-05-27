using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Web.Pages;

public class CollectionDetailModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public Collection? Collection { get; set; }
    public List<Domain.Models.Product> Products { get; set; } = new();

    public CollectionDetailModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return RedirectToPage("/Collections");
        }

        Collection = await _context.Collections
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive == true);

        if (Collection == null)
        {
            return RedirectToPage("/Collections");
        }

        Products = await _context.Products
            .Where(p => p.CollectionId == Collection.Id)
            .Include(p => p.ProductImages)
            .ToListAsync();

        return Page();
    }
}
