using System.Threading.Tasks;

namespace WarrantyTracker.Services
{
    public interface INotificationService
    {
        Task CheckWarrantyStatusesAsync(string userId);
    }
}
