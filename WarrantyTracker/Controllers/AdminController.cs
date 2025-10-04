using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WarrantyTracker.Data;
using WarrantyTracker.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WarrantyTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            // Get all users
            var users = await _userManager.Users.ToListAsync();

            // Assign role for each user (only 1 role per user in your case)
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.Count > 0 ? roles[0] : "No Role";
            }

            // Get appliances with user info
            var appliances = await _context.Appliances
                .Include(a => a.User) // navigation property: each appliance belongs to a user
                .ToListAsync();

            // Prepare model
            var model = new AdminDashboardViewModel
            {
                Users = users,
                TotalUsers = users.Count,
                TotalAppliances = appliances.Count,
                TotalServiceRecords = await _context.ServiceRecords.CountAsync(),
                UserRoles = userRoles,
                Appliances = appliances
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

       
        public async Task<IActionResult> ViewAppliance(int id)
        {
            var appliance = await _context.Appliances
                .Include(a => a.User) // load associated user
                .Include(a => a.ServiceRecords) // load associated services
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appliance == null)
                return NotFound();

            return View(appliance);
        }


        // Optional: delete appliance
        [HttpPost]
        public async Task<IActionResult> DeleteAppliance(int id)
        {
            var appliance = await _context.Appliances.FindAsync(id);
            if (appliance != null)
            {
                _context.Appliances.Remove(appliance);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }

    // ViewModel
    public class AdminDashboardViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAppliances { get; set; }
        public int TotalServiceRecords { get; set; }

        // one role per user
        public Dictionary<string, string> UserRoles { get; set; } = new Dictionary<string, string>();

        // appliances with users
        public List<Appliance> Appliances { get; set; } = new List<Appliance>();
    }
}
