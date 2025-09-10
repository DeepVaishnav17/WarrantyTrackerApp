using System;
using System.Collections.Generic;
using System.Linq;
using WarrantyTracker.Data;
using WarrantyTracker.Models;

namespace WarrantyTracker.Repositories
{
    public class SQLServiceRecordRepository : IServiceRecordRepository
    {
        private readonly ApplicationDbContext _db;

        public SQLServiceRecordRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<ServiceRecord> GetAll()
        {
            return _db.ServiceRecords
                      .OrderByDescending(s => s.ServiceDate)
                      .ToList();
        }

        public IEnumerable<ServiceRecord> GetByAppliance(int applianceId)
        {
            return _db.ServiceRecords
                      .Where(s => s.ApplianceId == applianceId)
                      .OrderByDescending(s => s.ServiceDate)
                      .ToList();
        }

        public ServiceRecord GetById(int id)
        {
            return _db.ServiceRecords
                      .SingleOrDefault(s => s.Id == id);
            // or _db.ServiceRecords.Find(id);
        }

        public void Add(ServiceRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            _db.ServiceRecords.Add(record);
            _db.SaveChanges();
        }

        public void Update(ServiceRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            var existing = _db.ServiceRecords.SingleOrDefault(s => s.Id == record.Id);
            if (existing == null) throw new InvalidOperationException("Service record not found.");

            existing.ServiceDate = record.ServiceDate;
            existing.VendorName = record.VendorName;
            existing.VendorContact = record.VendorContact;
            existing.Notes = record.Notes;
            existing.Cost = record.Cost;

            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var existing = _db.ServiceRecords.SingleOrDefault(s => s.Id == id);
            if (existing == null) return;

            _db.ServiceRecords.Remove(existing);
            _db.SaveChanges();
        }
    }
}
