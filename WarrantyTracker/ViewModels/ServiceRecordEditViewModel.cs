using System;
using System.ComponentModel.DataAnnotations;

namespace WarrantyTracker.ViewModels
{
    public class ServiceRecordEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ApplianceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Service Date")]
        public DateTime ServiceDate { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Vendor Contact")]
        public string VendorContact { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}
