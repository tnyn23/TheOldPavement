using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Web.Pages.Customer;

public class OrdersModel : PageModel
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;

    public List<Order> Orders { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public OrdersModel(IOrderRepository orderRepository, IOrderService orderService)
    {
        _orderRepository = orderRepository;
        _orderService = orderService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Public/Account/Login");
        }

        SuccessMessage = TempData["SuccessMessage"] as string;
        ErrorMessage = TempData["ErrorMessage"] as string;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            var dbOrders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (dbOrders != null)
            {
                Orders = dbOrders.OrderByDescending(o => o.CreatedAt).ToList();
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostCancelOrderAsync(int orderId)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Public/Account/Login");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            var result = await _orderService.CancelOrderAsync(orderId, userId);
            if (result)
            {
                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công. Tồn kho đã được hoàn lại.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng. Chỉ các đơn hàng đang chờ xử lý mới có thể hủy.";
            }
        }

        return RedirectToPage();
    }
}
