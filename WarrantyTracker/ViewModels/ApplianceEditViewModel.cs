using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WarrantyTracker.ViewModels
{
    public class ApplianceEditViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the appliance name.")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [Required(ErrorMessage = "Please select the purchase date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }

        [Range(0, 1200, ErrorMessage = "Warranty period must be between 0 and 1200 months.")]
        public int WarrantyPeriodMonths { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime WarrantyEndDate { get; set; }

        [Required(ErrorMessage = "Please provide a purchase price (enter 0 if free/gift).")]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal? PurchasePrice { get; set; }

        public string ReceiptImagePath { get; set; }

        public bool RemoveReceipt { get; set; }

        // NEW: file upload property so Razor helpers can bind & validate
        [DataType(DataType.Upload)]
        public IFormFile ReceiptFile { get; set; }
    }
}
