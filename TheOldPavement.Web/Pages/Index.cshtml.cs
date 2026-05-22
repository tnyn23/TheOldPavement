using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheOldPavement.Core.Interfaces;
using TheOldPavement.Core.Models;
using TheOldPavement.Application.DTOs;

namespace TheOldPavement.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<TheOldPavement.Core.Models.Product> FeaturedProducts { get; set; } = new List<TheOldPavement.Core.Models.Product>();

    public IndexModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync()
    {
        // For now, get all products or featured ones
        FeaturedProducts = await _productRepository.GetFeaturedProductsAsync();
        
        // If empty, generate some fake ones for the UI to look good
        if (!FeaturedProducts.Any())
        {
            FeaturedProducts = new List<TheOldPavement.Core.Models.Product>
            {
                new TheOldPavement.Core.Models.Product { Id = 1, Name = "36 PHỐ PHƯỜNG - HỒN HÀ NỘI TEE WHITE", Price = 425000, Slug = "36-pho-phuong" },
                new TheOldPavement.Core.Models.Product { Id = 2, Name = "36 PHỐ PHƯỜNG - HỒN HÀ NỘI TEE BLACK", Price = 425000, Slug = "36-pho-phuong" },
                new TheOldPavement.Core.Models.Product { Id = 3, Name = "36 PHỐ PHƯỜNG - HỒN HÀ NỘI TEE GRAY", Price = 425000, Slug = "36-pho-phuong" },
                new TheOldPavement.Core.Models.Product { Id = 4, Name = "THE OLD PAVEMENT LOGO TEE WHITE", Price = 399000, Slug = "logo-tee-white" },
            };
        }
    }

    public IActionResult OnPostQuickAdd(int productId, string productName, decimal price, string slug)
    {
        var cartItem = new CartItemDTO
        {
            ProductId = productId,
            VariantId = 1, // Default variant
            ProductName = productName,
            UnitPrice = price,
            Quantity = 1,
            Size = "L",
            Color = "white",
            ProductThumbnail = "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx3aGl0ZSUyMGJsYW5rJTIwdC1zaGlydCUyMG1vY2t1cCUyMGZsYXQlMjBsYXl8ZW58MXx8fHwxNzczNjc0Mzg3fDA&ixlib=rb-4.1.0&q=80&w=1080"
        };

        TheOldPavement.Web.Helpers.CartManager.AddToCart(HttpContext.Session, cartItem);
        TempData["SuccessMessage"] = $"Đã thêm {productName} vào giỏ hàng thành công!";
        return RedirectToPage();
    }
}
