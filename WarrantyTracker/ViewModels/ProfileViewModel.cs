using System.ComponentModel.DataAnnotations;

namespace WarrantyTracker.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number starting with 6-9")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
