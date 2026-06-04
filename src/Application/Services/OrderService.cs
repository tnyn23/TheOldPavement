using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<ProductVariant> _variantRepository;
    private readonly INotificationService _notificationService;

    public OrderService(IOrderRepository orderRepository, IRepository<ProductVariant> variantRepository, INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _variantRepository = variantRepository;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
        return orders.Select(MapToDto);
    }

    public Task<int> CreateOrderAsync(CreateOrderDTO dto)
    {
        // Checkout flow is handled by CheckoutService
        throw new NotImplementedException("Use CheckoutService.ProcessCheckoutAsync instead.");
    }

    public async Task UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order != null)
        {
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            
            if (order.UserId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    order.UserId.Value,
                    "order_update",
                    "Cập nhật đơn hàng",
                    $"Đơn hàng {order.OrderNumber} của bạn đã chuyển sang trạng thái: {status}",
                    $"/User/Orders/{order.Id}");
            }
        }
    }

    public async Task<bool> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(orderId);

        if (order == null || order.UserId != userId)
            return false;

        // Only allow cancellation of pending orders
        if (order.Status?.ToLower() != "pending")
            return false;

        order.Status = "cancelled";
        order.UpdatedAt = DateTime.UtcNow;

        // Rollback inventory
        foreach (var item in order.OrderItems)
        {
            if (item.VariantId > 0)
            {
                var variant = await _variantRepository.GetByIdAsync(item.VariantId);
                if (variant != null)
                {
                    variant.StockQuantity = (variant.StockQuantity ?? 0) + item.Quantity;
                    variant.IsAvailable = true;
                    await _variantRepository.UpdateAsync(variant);
                }
            }
        }

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return true;
    }

    private OrderDTO MapToDto(Order order)
    {
        return new OrderDTO
        {
            Id = order.Id,
            UserId = order.UserId ?? 0,
            OrderCode = order.OrderNumber,
            TotalAmount = order.Subtotal,
            DiscountAmount = order.DiscountAmount ?? 0,
            FinalAmount = order.TotalAmount,
            Status = order.Status ?? "pending",
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus ?? "pending",
            Note = order.Note,
            CreatedAt = order.CreatedAt ?? DateTime.Now,
            Items = order.OrderItems.Select(i => new OrderItemDTO
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Subtotal
            }).ToList()
        };
    }
}
