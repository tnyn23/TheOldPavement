using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Context;

namespace Web.Pages.User;

public class OrdersModel : PageModel
{
    private readonly TheOldPavementDbContext _context;

    public OrdersModel(TheOldPavementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order != null)
        {
            return RedirectToPage("/Customer/Orders", new { search = order.OrderNumber });
        }
        return RedirectToPage("/Customer/Orders");
    }
}
