using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Interfaces;
using Domain.Models;

namespace Web.Pages;

public class RollingStonesModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<Domain.Models.Product> Products { get; set; } = new List<Domain.Models.Product>();

    public RollingStonesModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync()
    {
        Products = await _productRepository.GetFeaturedProductsAsync();
    }
}


