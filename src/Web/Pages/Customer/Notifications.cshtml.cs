using System.Security.Claims;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Customer;

[Authorize]
public class NotificationsModel : PageModel
{
    private readonly INotificationService _notificationService;

    public NotificationsModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public List<Notification> Notifications { get; set; } = new List<Notification>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
        {
            return RedirectToPage("/Public/Account/Login");
        }

        Notifications = await _notificationService.GetUserNotificationsAsync(userId, 50);

        // Mark as read after viewing
        foreach (var notif in Notifications.Where(n => n.IsRead != true))
        {
            await _notificationService.MarkAsReadAsync(notif.Id);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkAllAsReadAsync()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out int userId))
        {
            return RedirectToPage("/Public/Account/Login");
        }

        var unreadNotifs = await _notificationService.GetUserNotificationsAsync(userId, 100);
        foreach (var notif in unreadNotifs.Where(n => n.IsRead != true))
        {
            await _notificationService.MarkAsReadAsync(notif.Id);
        }

        return RedirectToPage();
    }
}
