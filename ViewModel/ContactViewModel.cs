using System.ComponentModel.DataAnnotations;
using TestAjax.Models;
namespace TestAjax.ViewMdoel
{
    public class ContactViewModel
    {

        [Required(ErrorMessage = "Contact name is required")]
        [StringLength(150)]
        [Display(Name = "Contact name")]
        public string Name { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone number (optional)")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Contact type")]
        public string ContactType { get; set; } = "Customer"; // Customer or Supplier

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}