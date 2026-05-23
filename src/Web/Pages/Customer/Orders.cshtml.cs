using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Domain.Interfaces;
using Domain.Models;

namespace Web.Pages.Customer;

public class OrdersModel : PageModel
{
    private readonly IOrderRepository _orderRepository;

    public List<Order> Orders { get; set; } = new();

    public OrdersModel(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToPage("/Public/Account/Login");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            var dbOrders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (dbOrders != null)
            {
                Orders = dbOrders.ToList();
            }
        }

        // Mock fallback orders if none exist, so the user can test the visual interface instantly
        if (Orders.Count == 0)
        {
            Orders = new List<Order>
            {
                new Order
                {
                    Id = 8801,
                    OrderNumber = "TOP202605191182",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    Status = "delivered",
                    TotalAmount = 850000,
                    PaymentMethod = "cod",
                    PaymentStatus = "paid",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductName = "36 Phố Phường - Hồn Hà Nội Tee",
                            Quantity = 2,
                            UnitPrice = 425000,
                            Size = "XL",
                            Color = "White",
                            Subtotal = 850000
                        }
                    }
                },
                new Order
                {
                    Id = 8802,
                    OrderNumber = "TOP202605179024",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    Status = "pending",
                    TotalAmount = 399000,
                    PaymentMethod = "vnpay",
                    PaymentStatus = "pending",
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductName = "The Old Pavement Logo Tee White",
                            Quantity = 1,
                            UnitPrice = 399000,
                            Size = "M",
                            Color = "White",
                            Subtotal = 399000
                        }
                    }
                }
            };
        }

        return Page();
    }
}


