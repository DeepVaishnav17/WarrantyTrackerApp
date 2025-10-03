using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WarrantyTracker.Hubs
{
    public class NotificationHub : Hub
    {
        // Optional: method to send message to all clients
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public override Task OnConnectedAsync()
        {
            // Optionally: Log connected user
            // var user = Context.UserIdentifier;
            return base.OnConnectedAsync();
        }
    }
}
