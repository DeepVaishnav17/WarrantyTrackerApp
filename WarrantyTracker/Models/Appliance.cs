using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarrantyTracker.Models
{
    public class Appliance
    {
        public int Id { get; set; }

        // FK to AspNetUsers
        [Required(ErrorMessage = "User association is required.")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Please enter the appliance name.")]
        [StringLength(100, ErrorMessage = "Appliance name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Brand name cannot be longer than 50 characters.")]
        public string Brand { get; set; }

        [StringLength(50, ErrorMessage = "Model name cannot be longer than 50 characters.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Please select the purchase date.")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        // Not stored in DB — used on the form to get period in months
        [NotMapped]
        [Required(ErrorMessage = "Please enter warranty period in months.")]
        [Range(0, 1200, ErrorMessage = "Warranty period must be between 0 and 1200 months.")]
        public int WarrantyPeriodMonths { get; set; }

        // Stored in DB — computed on server before save
        [DataType(DataType.Date)]
        public DateTime WarrantyEndDate { get; set; }

        [Required(ErrorMessage = "Please provide a purchase price (enter 0 if free/gift).")]
        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be 0 or a positive value.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PurchasePrice { get; set; }

        public string ReceiptImagePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ServiceRecord> ServiceRecords { get; set; }
    }
}
