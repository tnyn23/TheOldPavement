using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Web.Hubs;

namespace Web.Services;

public class SignalRNotificationDispatcher : INotificationDispatcher
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<SignalRNotificationDispatcher> _logger;

    public SignalRNotificationDispatcher(IHubContext<NotificationHub> hubContext, ILogger<SignalRNotificationDispatcher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationAsync(int userId, Notification notification)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                type = notification.Type,
                link = notification.Link,
                createdAt = notification.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push SignalR notification to user {UserId}", userId);
        }
    }

    public async Task NotifyAdminNewOrderAsync(string orderNumber, decimal totalAmount)
    {
        try
        {
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveAdminNotification", new
            {
                type = "new_order",
                orderNumber = orderNumber,
                totalAmount = totalAmount,
                createdAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push SignalR notification to admins");
        }
    }
}
