using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WarrantyTracker.Models;
using WarrantyTracker.Services;

namespace WarrantyTracker.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> CheckNotifications()
        {
            var userId = _userManager.GetUserId(User);
            await _notificationService.CheckWarrantyStatusesAsync(userId);
            return Ok();
        }
    }
}
