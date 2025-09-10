using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WarrantyTracker.Models;
using WarrantyTracker.Repositories;
using WarrantyTracker.ViewModels;

namespace WarrantyTracker.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class AppliancesController : Controller
    {
        private readonly IApplianceRepository _applianceRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AppliancesController(
            IApplianceRepository applianceRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _applianceRepo = applianceRepo;
            _userManager = userManager;
            _env = env;
        }

        // GET: /Appliances
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var appliances = _applianceRepo.GetAllByUser(userId);
            return View(appliances);
        }

        // GET: /Appliances/Details/5
        public IActionResult Details(int id)
        {
            var appliance = _applianceRepo.GetById(id);
            if (appliance == null) return NotFound();

            // owner check
            var userId = _userManager.GetUserId(User);
            if (appliance.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(appliance);
        }

        // GET: /Appliances/Create
        public IActionResult Create()
        {
            var vm = new ApplianceCreateViewModel
            {
                PurchaseDate = DateTime.UtcNow.Date
            };
            return View(vm);
        }

        // POST: /Appliances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplianceCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Map VM -> entity
            var appliance = new Appliance
            {
                Name = vm.Name,
                Brand = vm.Brand,
                Model = vm.Model,
                PurchaseDate = vm.PurchaseDate,
                PurchasePrice = vm.PurchasePrice,
                // WarrantyEndDate will be computed below if months provided
                WarrantyEndDate = vm.WarrantyEndDate
            };

            // owner
            appliance.UserId = _userManager.GetUserId(User);

            // compute warranty end date if months provided
            if (vm.WarrantyPeriodMonths > 0)
            {
                appliance.WarrantyEndDate = vm.PurchaseDate.AddMonths(vm.WarrantyPeriodMonths);
            }

            // Receipt file validation & save (same rules as Edit)
            var receiptFile = vm.ReceiptFile;
            if (receiptFile != null && receiptFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var ext = Path.GetExtension(receiptFile.FileName).ToLowerInvariant();
                const long maxBytes = 5 * 1024 * 1024; // 5 MB

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.ReceiptFile), "Only JPG, PNG or PDF files are allowed.");
                    return View(vm);
                }

                if (receiptFile.Length > maxBytes)
                {
                    ModelState.AddModelError(nameof(vm.ReceiptFile), "File too large (max 5 MB).");
                    return View(vm);
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "receipts");
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    receiptFile.CopyTo(stream);
                }

                appliance.ReceiptImagePath = Path.Combine("/uploads/receipts", fileName).Replace('\\', '/');
            }

            // repository Add will set CreatedAt & UpdatedAt (ensure your Add method does that)
            _applianceRepo.Add(appliance);

            TempData["SuccessMessage"] = "Appliance created successfully.";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Appliances/Edit/5
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

        // POST: /Appliances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplianceEditViewModel vm, IFormFile receiptFile)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existing = _applianceRepo.GetById(vm.Id);
            if (existing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (existing.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // compute warranty end date
            DateTime computedEnd;
            if (vm.WarrantyPeriodMonths > 0)
                computedEnd = vm.PurchaseDate.AddMonths(vm.WarrantyPeriodMonths);
            else
                computedEnd = vm.WarrantyEndDate;

            if (computedEnd < vm.PurchaseDate)
            {
                ModelState.AddModelError(nameof(vm.WarrantyEndDate),
                    "Warranty end date cannot be earlier than purchase date.");
                vm.ReceiptImagePath = existing.ReceiptImagePath;
                return View(vm);
            }

            // apply changes to tracked entity
            existing.Name = vm.Name;
            existing.Brand = vm.Brand;
            existing.Model = vm.Model;
            existing.PurchaseDate = vm.PurchaseDate;
            existing.WarrantyEndDate = computedEnd;
            existing.PurchasePrice = vm.PurchasePrice;
            existing.UpdatedAt = DateTime.UtcNow;

            var oldReceiptPath = existing.ReceiptImagePath;

            if (vm.RemoveReceipt)
            {
                if (!string.IsNullOrEmpty(oldReceiptPath))
                {
                    try
                    {
                        var oldPhysical = Path.Combine(_env.WebRootPath ?? "wwwroot",
                            oldReceiptPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPhysical)) System.IO.File.Delete(oldPhysical);
                    }
                    catch { }
                }
                existing.ReceiptImagePath = null;
            }

            // receipt validation & save (same checks as Create)
            if (receiptFile != null && receiptFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var ext = Path.GetExtension(receiptFile.FileName).ToLowerInvariant();
                const long maxBytes = 5 * 1024 * 1024;

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("receiptFile", "Only JPG, PNG or PDF files are allowed.");
                    vm.ReceiptImagePath = existing.ReceiptImagePath;
                    return View(vm);
                }

                if (receiptFile.Length > maxBytes)
                {
                    ModelState.AddModelError("receiptFile", "File too large (max 5 MB).");
                    vm.ReceiptImagePath = existing.ReceiptImagePath;
                    return View(vm);
                }

                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "receipts");
                if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    receiptFile.CopyTo(stream);
                }
                existing.ReceiptImagePath = Path.Combine("/uploads/receipts", fileName).Replace('\\', '/');

                // delete old file (best-effort)
                if (!string.IsNullOrEmpty(oldReceiptPath))
                {
                    try
                    {
                        var oldPhysical = Path.Combine(_env.WebRootPath ?? "wwwroot",
                            oldReceiptPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldPhysical)) System.IO.File.Delete(oldPhysical);
                    }
                    catch { }
                }
            }

            // use existing Update method in repository (it handles UpdatedAt too)
            _applianceRepo.Update(existing);

            TempData["SuccessMessage"] = "Appliance updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private int CalculateMonths(DateTime start, DateTime end)
        {
            if (end < start) return 0;
            int months = (end.Year - start.Year) * 12 + end.Month - start.Month;
            if (end.Day < start.Day) months--;
            return Math.Max(0, months);
        }

        // GET: /Appliances/Delete/5
        public IActionResult Delete(int id)
        {
            var appliance = _applianceRepo.GetById(id);
            if (appliance == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (appliance.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(appliance); // confirmation page
        }

        // POST: /Appliances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var existing = _applianceRepo.GetById(id);
            if (existing == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (existing.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            // delete receipt file
            if (!string.IsNullOrEmpty(existing.ReceiptImagePath))
            {
                try
                {
                    var old = Path.Combine(_env.WebRootPath ?? "wwwroot", existing.ReceiptImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }
                catch { }
            }

            _applianceRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
