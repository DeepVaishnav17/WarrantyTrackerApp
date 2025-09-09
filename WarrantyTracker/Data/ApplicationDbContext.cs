using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarrantyTracker.Models;

namespace WarrantyTracker.Data
{
    // IdentityDbContext includes AspNetUsers, AspNetRoles, etc.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //// Your custom tables
        //public DbSet<Appliance> Appliances { get; set; }
        //public DbSet<ServiceRecord> ServiceRecords { get; set; }
        //public DbSet<Notification> Notifications { get; set; }
    }
}
