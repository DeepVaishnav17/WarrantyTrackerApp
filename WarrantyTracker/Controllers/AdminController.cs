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
            var users = await _userManager.Users.ToListAsync();

            var model = new AdminDashboardViewModel
            {
                Users = users,
                TotalUsers = users.Count,
                TotalAppliances = await _context.Appliances.CountAsync(),
                TotalServiceRecords = await _context.ServiceRecords.CountAsync()
            };

            return View(model);
        }
    }

    // Simple VM for admin dashboard
    public class AdminDashboardViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAppliances { get; set; }
        public int TotalServiceRecords { get; set; }
    }
}
