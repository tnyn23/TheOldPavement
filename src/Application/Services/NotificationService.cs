using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly INotificationDispatcher _dispatcher;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IRepository<Notification> notificationRepository, INotificationDispatcher dispatcher, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task<Notification> CreateNotificationAsync(int userId, string type, string title, string message, string? link = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Link = link,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();
        
        await _dispatcher.SendNotificationAsync(userId, notification);

        return notification;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 20)
    {
        var all = await _notificationRepository.GetAllAsync();
        return all.Where(n => n.UserId == userId)
                  .OrderByDescending(n => n.CreatedAt)
                  .Take(limit)
                  .ToList();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }
        return false;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        var all = await _notificationRepository.GetAllAsync();
        return all.Count(n => n.UserId == userId && n.IsRead != true);
    }
}
