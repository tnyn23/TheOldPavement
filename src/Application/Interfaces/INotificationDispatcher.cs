using Domain.Models;

namespace Application.Interfaces;

public interface INotificationDispatcher
{
    Task SendNotificationAsync(int userId, Notification notification);
}
