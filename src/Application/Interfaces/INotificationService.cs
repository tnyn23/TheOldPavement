using Domain.Models;

namespace Application.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(int userId, string type, string title, string message, string? link = null);
    Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 20);
    Task<bool> MarkAsReadAsync(int notificationId);
    Task<int> GetUnreadCountAsync(int userId);
}
