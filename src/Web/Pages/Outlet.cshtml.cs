using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Interfaces;
using Domain.Models;

namespace Web.Pages;

public class OutletModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<Domain.Models.Product> Products { get; set; } = new List<Domain.Models.Product>();
    public string SelectedCondition { get; set; } = "all";

    public OutletModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync(string? condition)
    {
        if (!string.IsNullOrEmpty(condition))
        {
            SelectedCondition = condition.ToLower();
        }

        Products = await _productRepository.GetFeaturedProductsAsync();
    }
}


