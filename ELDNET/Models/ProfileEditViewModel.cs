
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Add this using directive

namespace ELDNET.Models
{
    public class ProfileEditViewModel
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public string Role { get; set; }

        [Display(Name = "Current Profile Picture")]
        public string? ProfilePictureUrl { get; set; }

        // New property for file upload
        [Display(Name = "Upload New Profile Picture")]
        public IFormFile? ProfileImage { get; set; } // Make it nullable
    }
}