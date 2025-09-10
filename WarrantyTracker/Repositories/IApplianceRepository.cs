using System.Collections.Generic;
using WarrantyTracker.Models;

namespace WarrantyTracker.Repositories
{
    public interface IApplianceRepository
    {
        IEnumerable<Appliance> GetAll();                         // admin: all appliances
        IEnumerable<Appliance> GetAllByUser(string userId);      // user's appliances
        Appliance GetById(int id);
        void Add(Appliance appliance);
        void Update(Appliance appliance);
        void Delete(int id);

        // helper queries
        IEnumerable<Appliance> GetExpiringWithinDays(string userId, int days);
        IEnumerable<Appliance> GetExpired(string userId);
        void UpdateTracked(Appliance existing);
    }
}
