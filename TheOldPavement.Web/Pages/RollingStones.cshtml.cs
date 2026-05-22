using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Core.Interfaces;
using TheOldPavement.Core.Models;

namespace TheOldPavement.Web.Pages;

public class RollingStonesModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<TheOldPavement.Core.Models.Product> Products { get; set; } = new List<TheOldPavement.Core.Models.Product>();

    public RollingStonesModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync()
    {
        Products = await _productRepository.GetFeaturedProductsAsync();
    }
}
