using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Domain.Interfaces;
using TheOldPavement.Domain.Models;

namespace TheOldPavement.Web.Pages;

public class RollingStonesModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<TheOldPavement.Domain.Models.Product> Products { get; set; } = new List<TheOldPavement.Domain.Models.Product>();

    public RollingStonesModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync()
    {
        Products = await _productRepository.GetFeaturedProductsAsync();
    }
}

