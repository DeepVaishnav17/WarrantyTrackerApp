using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WarrantyTracker.Hubs;
using WarrantyTracker.Models;
using WarrantyTracker.Repositories;
using WarrantyTracker.ViewModels;

namespace WarrantyTracker.Controllers
{
    [Authorize(Roles = "User")]
    public class AppliancesController : Controller
    {
        private readonly IApplianceRepository _applianceRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AppliancesController(
            IApplianceRepository applianceRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IHubContext<NotificationHub> hubContext)
        {
            _applianceRepo = applianceRepo;
            _userManager = userManager;
            _env = env;
            _hubContext = hubContext;
        }

        // ===============================
        // Index
        // ===============================
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var appliances = _applianceRepo.GetAllByUser(userId);
            return View(appliances);
        }

        // ===============================
        // Details
        // ===============================
        public IActionResult Details(int id)
        {
            var appliance = _applianceRepo.GetById(id);
            if (appliance == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (appliance.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(appliance);
        }

        // ===============================
        // Create
        // ===============================
        public IActionResult Create()
        {
            var vm = new ApplianceCreateViewModel
            {
                PurchaseDate = DateTime.UtcNow.Date
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplianceCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var appliance = new Appliance
            {
                Name = vm.Name,
                Brand = vm.Brand,
                Model = vm.Model,
                PurchaseDate = vm.PurchaseDate,
                PurchasePrice = vm.PurchasePrice,
                UserId = _userManager.GetUserId(User)
            };

            // calculate warranty end
            appliance.WarrantyEndDate = CalculateWarrantyEndDate(vm.PurchaseDate, vm.WarrantyPeriodMonths);

            // validate and save receipt
            appliance.ReceiptImagePath = SaveReceipt(vm.ReceiptFile);

            // set warranty status immediately
            appliance.LastWarrantyStatus = Appliance.CalculateWarrantyStatus(appliance.WarrantyEndDate, vm.WarrantyPeriodMonths);

            _applianceRepo.Add(appliance);
            TempData["SuccessMessage"] = "Appliance created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // Edit
        // ===============================
        public IActionResult Edit(int id)
        {
            var appliance = _applianceRepo.GetById(id);
            if (appliance == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (appliance.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var vm = new ApplianceEditViewModel
            {
                Id = appliance.Id,
                Name = appliance.Name,
                Brand = appliance.Brand,
                Model = appliance.Model,
                PurchaseDate = appliance.PurchaseDate,
                PurchasePrice = appliance.PurchasePrice,
                ReceiptImagePath = appliance.ReceiptImagePath,
                WarrantyEndDate = appliance.WarrantyEndDate,
                WarrantyPeriodMonths = CalculateMonths(appliance.PurchaseDate, appliance.WarrantyEndDate)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplianceEditViewModel vm, IFormFile receiptFile)
        {
            if (!ModelState.IsValid) return View(vm);

            var existing = _applianceRepo.GetById(vm.Id);
            if (existing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (existing.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var oldStatus = existing.LastWarrantyStatus;
            var oldEndDate = existing.WarrantyEndDate;

            // update fields
            existing.Name = vm.Name;
            existing.Brand = vm.Brand;
            existing.Model = vm.Model;
            existing.PurchaseDate = vm.PurchaseDate;
            existing.PurchasePrice = vm.PurchasePrice;
            existing.WarrantyEndDate = CalculateWarrantyEndDate(vm.PurchaseDate, vm.WarrantyPeriodMonths);
            existing.UpdatedAt = DateTime.UtcNow;

            // receipt handling
            if (vm.RemoveReceipt)
            {
                DeleteFile(existing.ReceiptImagePath);
                existing.ReceiptImagePath = null;
            }
            else if (receiptFile != null && receiptFile.Length > 0)
            {
                DeleteFile(existing.ReceiptImagePath); // remove old
                existing.ReceiptImagePath = SaveReceipt(receiptFile);
            }

            // update warranty status
            existing.LastWarrantyStatus = Appliance.CalculateWarrantyStatus(existing.WarrantyEndDate, vm.WarrantyPeriodMonths);

            _applianceRepo.Update(existing);

            // send notification if something changed
            if (oldStatus != existing.LastWarrantyStatus || oldEndDate != existing.WarrantyEndDate)
            {
                string message = $"{existing.Name} warranty updated: {oldEndDate:dd MMM yyyy} → {existing.WarrantyEndDate:dd MMM yyyy} ({existing.LastWarrantyStatus})";
                await _hubContext.Clients.User(existing.UserId).SendAsync("ReceiveNotification", message);
            }

            TempData["SuccessMessage"] = "Appliance updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // Delete
        // ===============================
        public IActionResult Delete(int id)
        {
            var appliance = _applianceRepo.GetById(id);
            if (appliance == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (appliance.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(appliance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var existing = _applianceRepo.GetById(id);
            if (existing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (existing.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            DeleteFile(existing.ReceiptImagePath);
            _applianceRepo.Delete(id);

            TempData["SuccessMessage"] = "Appliance deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // Helpers
        // ===============================

        private DateTime CalculateWarrantyEndDate(DateTime purchaseDate, int months)
        {
            if (months > 0)
                return purchaseDate.AddMonths(months);

            // if no warranty → store purchase date as end date
            return purchaseDate;
        }

        private int CalculateMonths(DateTime start, DateTime end)
        {
            if (end < start) return 0;
            int months = (end.Year - start.Year) * 12 + end.Month - start.Month;
            if (end.Day < start.Day) months--;
            return Math.Max(0, months);
        }

        private string SaveReceipt(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            const long maxBytes = 5 * 1024 * 1024;

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("ReceiptFile", "Only JPG, PNG or PDF files are allowed.");
                return null;
            }

            if (file.Length > maxBytes)
            {
                ModelState.AddModelError("ReceiptFile", "File too large (max 5 MB).");
                return null;
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "receipts");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Path.Combine("/uploads/receipts", fileName).Replace('\\', '/');
        }

        private void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? "wwwroot", path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }
            catch { }
        }
    }
}
