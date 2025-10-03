using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WarrantyTracker.Hubs;
using WarrantyTracker.Models;
using WarrantyTracker.Repositories;

namespace WarrantyTracker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApplianceRepository _applianceRepo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IApplianceRepository applianceRepo, IHubContext<NotificationHub> hubContext)
        {
            _applianceRepo = applianceRepo;
            _hubContext = hubContext;
        }

        public async Task CheckWarrantyStatusesAsync(string userId)
        {
            var appliances = _applianceRepo.GetAllByUser(userId).ToList();
            var today = DateTime.UtcNow.Date;

            foreach (var appliance in appliances)
            {
                string newStatus;
                var daysLeft = (int)(appliance.WarrantyEndDate - today).TotalDays;

                if (daysLeft > 31) newStatus = "Active";
                else if (daysLeft >= 0) newStatus = "Expiring Soon";
                else newStatus = "Expired";

                if (appliance.LastWarrantyStatus != newStatus && newStatus != "Active")
                {
                    string message = newStatus == "Expiring Soon"
                        ? $"{appliance.Name} warranty is expiring soon!"
                        : $"{appliance.Name} warranty has expired!";

                    await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
                }

                appliance.LastWarrantyStatus = newStatus;
                _applianceRepo.Update(appliance);
            }
        }

        public static void NotifyIfStatusChanged(Appliance appliance)
        {
            if (appliance.LastWarrantyStatus == "Expiring Soon")
            {
                // Create and save notification in DB
                // Or push via SignalR to React frontend
            }
        }
        public async Task NotifyUserAsync(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
