using System;
using System.ComponentModel.DataAnnotations;

namespace WarrantyTracker.ViewModels
{
    public class ServiceRecordCreateViewModel
    {
        [Required]
        public int ApplianceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Service Date")]
        public DateTime ServiceDate { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Vendor name must be between 2 and 100 characters.")]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Enter a valid contact number (10–15 digits, optional + at start).")]
        [Display(Name = "Contact Number")]
        public string VendorContact { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Cost must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}
