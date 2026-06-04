using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Web.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            // Join a group for the specific user
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userIdClaim.Value}");
        }

        if (Context.User?.IsInRole("admin") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userIdClaim.Value}");
        }

        if (Context.User?.IsInRole("admin") == true)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
