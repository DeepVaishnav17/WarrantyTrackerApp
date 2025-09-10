using System.Collections.Generic;
using WarrantyTracker.Models;

namespace WarrantyTracker.Repositories
{
    public interface IServiceRecordRepository
    {
        IEnumerable<ServiceRecord> GetAll();                      // optional admin
        IEnumerable<ServiceRecord> GetByAppliance(int applianceId);
        ServiceRecord GetById(int id);
        void Add(ServiceRecord record);
        void Update(ServiceRecord record);
        void Delete(int id);
    }
}
