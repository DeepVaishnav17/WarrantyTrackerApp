using System.ComponentModel.DataAnnotations;


namespace WarrantyTracker.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}