using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;

namespace TheOldPavement.Web.Pages;

public class OutletModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<TheOldPavement.Domain.Models.Product> Products { get; set; } = new List<TheOldPavement.Domain.Models.Product>();
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

