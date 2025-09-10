using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarrantyTracker.Models
{
    public class ServiceRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Associated appliance is required.")]
        public int ApplianceId { get; set; }
        public Appliance Appliance { get; set; }

        [Required(ErrorMessage = "Please select the service date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Service Date")]
        public DateTime ServiceDate { get; set; }

        [Required(ErrorMessage = "Please enter the vendor name.")]
        [StringLength(100, ErrorMessage = "Vendor name cannot exceed 100 characters.")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Please enter the vendor contact number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Vendor Contact")]
        public string VendorContact { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string Notes { get; set; } // optional

        [Required(ErrorMessage = "Please enter the service cost.")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be 0 or a positive value.")]
        [Display(Name = "Cost")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
    }
}
