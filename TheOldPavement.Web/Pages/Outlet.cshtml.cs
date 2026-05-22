using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Core.Interfaces;
using TheOldPavement.Core.Models;

namespace TheOldPavement.Web.Pages;

public class OutletModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<TheOldPavement.Core.Models.Product> Products { get; set; } = new List<TheOldPavement.Core.Models.Product>();
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
