using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WarrantyTracker.Data;
using WarrantyTracker.Models;

namespace WarrantyTracker.Repositories
{
    public class SQLApplianceRepository : IApplianceRepository
    {
        private readonly ApplicationDbContext _db;

        public SQLApplianceRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Appliance> GetAll()
        {
            return _db.Appliances
                      .OrderByDescending(a => a.CreatedAt)
                      .ToList();
        }

        public IEnumerable<Appliance> GetAllByUser(string userId)
        {
            return _db.Appliances
                      .Where(a => a.UserId == userId)
                      .OrderByDescending(a => a.CreatedAt)
                      .ToList();
        }

        public Appliance GetById(int id)
        {
            return _db.Appliances
                .Include(a => a.ServiceRecords)   // 👈 eager load related records
                .FirstOrDefault(a => a.Id == id);
        }

        public void Add(Appliance appliance)
        {
            if (appliance == null) throw new ArgumentNullException(nameof(appliance));

            // compute warranty end date if WarrantyPeriodMonths provided
            if (appliance.WarrantyPeriodMonths > 0)
            {
                appliance.WarrantyEndDate = appliance.PurchaseDate.AddMonths(appliance.WarrantyPeriodMonths);
            }

            var now = DateTime.UtcNow;
            appliance.CreatedAt = now;
            appliance.UpdatedAt = now;

            _db.Appliances.Add(appliance);
            _db.SaveChanges();
        }

        public void Update(Appliance appliance)
        {
            if (appliance == null) throw new ArgumentNullException(nameof(appliance));

            var existing = _db.Appliances.SingleOrDefault(a => a.Id == appliance.Id);
            if (existing == null) throw new InvalidOperationException("Appliance not found.");

            // update only allowed fields (keep navigation & immutables safe)
            existing.Name = appliance.Name;
            existing.Brand = appliance.Brand;
            existing.Model = appliance.Model;
            existing.PurchaseDate = appliance.PurchaseDate;

            if (appliance.WarrantyPeriodMonths > 0)
            {
                existing.WarrantyEndDate = appliance.PurchaseDate.AddMonths(appliance.WarrantyPeriodMonths);
            }
            else
            {
                existing.WarrantyEndDate = appliance.WarrantyEndDate;
            }

            existing.PurchasePrice = appliance.PurchasePrice;
            existing.ReceiptImagePath = appliance.ReceiptImagePath;
            existing.UpdatedAt = DateTime.UtcNow;

            _db.SaveChanges();
        }


        public void Delete(int id)
        {
            var existing = _db.Appliances.SingleOrDefault(a => a.Id == id);
            if (existing == null) return; // nothing to delete

            // optional: delete related service records first (or rely on cascade delete)
            var services = _db.ServiceRecords.Where(s => s.ApplianceId == id).ToList();
            if (services.Any())
            {
                _db.ServiceRecords.RemoveRange(services);
            }

            _db.Appliances.Remove(existing);
            _db.SaveChanges();
        }

        public IEnumerable<Appliance> GetExpiringWithinDays(string userId, int days)
        {
            var now = DateTime.UtcNow.Date;
            var threshold = now.AddDays(days);

            return _db.Appliances
                      .Where(a => a.UserId == userId && a.WarrantyEndDate >= now && a.WarrantyEndDate <= threshold)
                      .OrderBy(a => a.WarrantyEndDate)
                      .ToList();
        }

        public IEnumerable<Appliance> GetExpired(string userId)
        {
            var now = DateTime.UtcNow.Date;
            return _db.Appliances
                      .Where(a => a.UserId == userId && a.WarrantyEndDate < now)
                      .OrderByDescending(a => a.WarrantyEndDate)
                      .ToList();
        }

        public void UpdateTracked(Appliance appliance)
        {
            if (appliance == null) throw new ArgumentNullException(nameof(appliance));
            _db.SaveChanges();
        }

    }
}
