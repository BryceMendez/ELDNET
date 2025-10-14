using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELDNET.Models
{
    public class LockerRequest
    {
        [Key] // Explicitly mark as Primary Key
        public int Id { get; set; }

        public string? StudentId { get; set; }

        public string? FacultyId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ID Number is required.")]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }

        [Required(ErrorMessage = "Locker Number is required.")]
        [Display(Name = "Locker Number")]
        public string LockerNumber { get; set; } // Kept as string for flexibility (e.g., "A-123")

        [Required(ErrorMessage = "Semester is required.")]
        public string Semester { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [Display(Name = "Contact Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")] // Optional: Add phone validation
        public string ContactNumber { get; set; }

        public string Status { get; set; } = "Pending"; // Default status

        // These store the paths in the database after upload.
        // They are not directly required from user input, the IFormFile handles that.
        [Display(Name = "Study Load")]
        public string? StudyLoadPath { get; set; }
        [Display(Name = "Registration Form")]
        public string? RegistrationPath { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now; // Default to current time for new requests

        // IFormFile properties for binding uploaded files
        // These are [Required] because the user must upload them.
        [NotMapped]
        [Required(ErrorMessage = "Please upload the Study Load file.")]
        [Display(Name = "Study Load")]
        public IFormFile? StudyLoadFile { get; set; } // Changed to nullable IFormFile to handle pre-existing files on Edit, but Required attribute still applies for new creations.

        [NotMapped]
        [Required(ErrorMessage = "Please upload the Registration Form file.")]
        [Display(Name = "Registration Form")]
        public IFormFile? RegistrationFile { get; set; } // Changed to nullable IFormFile

        [Required(ErrorMessage = "You must accept the terms and conditions.")]
        [Display(Name = "Accept Terms and Conditions")]
        public bool AcceptTerms { get; set; }

        public bool IsChangedByApplicant { get; set; } = false;

        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";
        public string FinalStatus { get; set; } = "Pending";
    }
}