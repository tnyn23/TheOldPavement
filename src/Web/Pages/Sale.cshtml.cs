using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Interfaces;
using Domain.Models;
using Application.DTOs;

namespace Web.Pages;

public class SaleModel : PageModel
{
    private readonly IProductRepository _productRepository;

    public IEnumerable<Domain.Models.Product> Products { get; set; } = new List<Domain.Models.Product>();
    public string SortBy { get; set; } = "discount";

    public SaleModel(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task OnGetAsync(string? sortBy)
    {
        if (!string.IsNullOrEmpty(sortBy))
        {
            SortBy = sortBy;
        }

        Products = await _productRepository.GetFeaturedProductsAsync();
    }

    public IActionResult OnPostQuickAdd(int productId, string productName, decimal price, string slug)
    {
        var cartItem = new CartItemDTO
        {
            ProductId = productId,
            VariantId = 1,
            ProductName = productName,
            UnitPrice = price,
            Quantity = 1,
            Size = "L",
            Color = "white",
            ProductThumbnail = "https://images.unsplash.com/photo-1651761179569-4ba2aa054997?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&q=80&w=1080"
        };

        Web.Helpers.CartManager.AddToCart(HttpContext.Session, cartItem);
        TempData["SuccessMessage"] = $"Đã thêm {productName} vào giỏ hàng thành công!";
        return RedirectToPage();
    }
}


