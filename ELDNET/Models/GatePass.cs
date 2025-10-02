using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        public int Id { get; set; }

        // Add this property to link to the student
        public string? StudentId { get; set; } // Foreign key to StudentAccount.StudentId

        [Required]
        [Display(Name = "School Year")]
        public string? SchoolYear { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string? Role { get; set; }

        public string? Department { get; set; }

        [Display(Name = "Course Year")]
        public string? CourseYear { get; set; }

        [Display(Name = "Plate Number")]
        public string? PlateNumber { get; set; }
        public string? Maker { get; set; }
        public string? Color { get; set; }
        public string? Status { get; set; } = "Pending";

        // New columns for file paths
        [Display(Name = "Study Load")]
        public string? StudyLoadPath { get; set; }
        [Display(Name = "Registration Form")]
        public string? RegistrationPath { get; set; }

        // Not mapped — only used for file upload
        [NotMapped]
        [Display(Name = "Study Load")]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        [Display(Name = "Registration Form")]
        public IFormFile? RegistrationFile { get; set; }
    }
}