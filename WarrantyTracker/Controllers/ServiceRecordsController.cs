using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarrantyTracker.Models;
using WarrantyTracker.Repositories;
using WarrantyTracker.ViewModels;

namespace WarrantyTracker.Controllers
{
    [Authorize]
    public class ServiceRecordsController : Controller
    {
        private readonly IServiceRecordRepository _serviceRepo;
        private readonly IApplianceRepository _applianceRepo;

        public ServiceRecordsController(IServiceRecordRepository serviceRepo, IApplianceRepository applianceRepo)
        {
            _serviceRepo = serviceRepo;
            _applianceRepo = applianceRepo;
        }

        // GET: /ServiceRecords/Index?applianceId=1
        public IActionResult Index(int applianceId)
        {
            var appliance = _applianceRepo.GetById(applianceId);
            if (appliance == null)
                return NotFound();

            ViewBag.Appliance = appliance;

            // fetch and order records (latest first)
            var records = _serviceRepo.GetByAppliance(applianceId)
                                      .OrderByDescending(r => r.ServiceDate)
                                      .ToList();

            return View(records);
        }


        // GET: /ServiceRecords/Create?applianceId=1
        public IActionResult Create(int applianceId)
        {
            var appliance = _applianceRepo.GetById(applianceId);
            if (appliance == null) return NotFound();

            ViewBag.PurchaseDate = appliance.PurchaseDate.ToString("dd-MM-yyyy");

            var vm = new ServiceRecordCreateViewModel
            {
                ApplianceId = applianceId,
                ServiceDate = DateTime.Today
            };

            return View(vm);
        }

        // POST: /ServiceRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceRecordCreateViewModel vm)
        {
            var appliance = _applianceRepo.GetById(vm.ApplianceId);
            if (appliance == null) return NotFound();

            var today = DateTime.Today;

            if (vm.ServiceDate > today)
                ModelState.AddModelError(nameof(vm.ServiceDate), "Service date cannot be in the future.");

            if (vm.ServiceDate < appliance.PurchaseDate.Date)
                ModelState.AddModelError(nameof(vm.ServiceDate), $"Service date cannot be earlier than appliance purchase date ({appliance.PurchaseDate:dd-MM-yyyy}).");

            if (vm.Cost <= 0)
                ModelState.AddModelError(nameof(vm.Cost), "Service cost must be greater than 0.");

            if (ModelState.IsValid)
            {
                var record = new ServiceRecord
                {
                    ApplianceId = vm.ApplianceId,
                    ServiceDate = vm.ServiceDate,
                    VendorName = vm.VendorName,
                    VendorContact = vm.VendorContact,
                    Cost = vm.Cost,
                    Notes = vm.Notes
                };

                _serviceRepo.Add(record);
                return RedirectToAction("Index", new { applianceId = vm.ApplianceId });
            }

            ViewBag.PurchaseDate = appliance.PurchaseDate.ToString("dd-MM-yyyy");
            return View(vm);
        }

        // GET: /ServiceRecords/Edit/5
        public IActionResult Edit(int id)
        {
            var record = _serviceRepo.GetById(id);
            if (record == null) return NotFound();

            var appliance = _applianceRepo.GetById(record.ApplianceId);
            if (appliance == null) return NotFound();

            ViewBag.PurchaseDate = appliance.PurchaseDate.ToString("dd-MM-yyyy");

            var vm = new ServiceRecordEditViewModel
            {
                Id = record.Id,
                ApplianceId = record.ApplianceId,
                ServiceDate = record.ServiceDate,
                VendorName = record.VendorName,
                VendorContact = record.VendorContact,
                Cost = record.Cost,
                Notes = record.Notes
            };

            return View(vm);
        }

        // POST: /ServiceRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ServiceRecordEditViewModel vm)
        {
            var appliance = _applianceRepo.GetById(vm.ApplianceId);
            if (appliance == null) return NotFound();

            var today = DateTime.Today;

            if (vm.ServiceDate > today)
                ModelState.AddModelError(nameof(vm.ServiceDate), "Service date cannot be in the future.");

            if (vm.ServiceDate < appliance.PurchaseDate.Date)
                ModelState.AddModelError(nameof(vm.ServiceDate), $"Service date cannot be earlier than appliance purchase date ({appliance.PurchaseDate:dd-MM-yyyy}).");

            if (vm.Cost <= 0)
                ModelState.AddModelError(nameof(vm.Cost), "Service cost must be greater than 0.");

            if (ModelState.IsValid)
            {
                var record = new ServiceRecord
                {
                    Id = vm.Id,
                    ApplianceId = vm.ApplianceId,
                    ServiceDate = vm.ServiceDate,
                    VendorName = vm.VendorName,
                    VendorContact = vm.VendorContact,
                    Cost = vm.Cost,
                    Notes = vm.Notes
                };

                _serviceRepo.Update(record);
                return RedirectToAction("Index", new { applianceId = vm.ApplianceId });
            }

            ViewBag.PurchaseDate = appliance.PurchaseDate.ToString("dd-MM-yyyy");
            return View(vm);
        }

        // GET: /ServiceRecords/Delete/5
        public IActionResult Delete(int id)
        {
            var record = _serviceRepo.GetById(id);
            if (record == null)
                return NotFound();

            return View(record);
        }

        // POST: /ServiceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var record = _serviceRepo.GetById(id);
            if (record == null)
                return NotFound();

            int applianceId = record.ApplianceId;
            _serviceRepo.Delete(id);
            return RedirectToAction("Index", new { applianceId });
        }
    }
}
