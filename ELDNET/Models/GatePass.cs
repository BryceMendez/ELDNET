using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        // No [Required] needed for Id, EF Core handles it
        [Key] // Explicitly mark as Primary Key
        public int Id { get; set; }

        public string? StudentId { get; set; }

        public string? FacultyId { get; set; }

        [Required]
        [Display(Name = "School Year")]
        public string SchoolYear { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Course Year")]
        public string CourseYear { get; set; }

        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [Required]
        public string Maker { get; set; }

        [Required]
        public string Color { get; set; }

        // [Required] is fine here, but default value already ensures it's not null/empty
        public string Status { get; set; } = "Pending";

        // Make paths nullable, remove [Required] as IFormFile will handle the "requiredness" of the upload
        [Display(Name = "Study Load")]
        public string? StudyLoadPath { get; set; } // Allow null initially

        [Display(Name = "Registration Form")]
        public string? RegistrationPath { get; set; } // Allow null initially

        [NotMapped]
        [Required(ErrorMessage = "Please upload the Study Load file.")] // Explicit error message
        [Display(Name = "Upload Study Load")]
        public IFormFile StudyLoadFile { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please upload the Registration Form file.")] // Explicit error message
        [Display(Name = "Upload Registration Form")]
        public IFormFile RegistrationFile { get; set; }

        public bool IsChangedByApplicant { get; set; } = false;

        // Approval Workflow Properties
        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "John Smith (Security Head)";
        public string Approver3Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";
    }
}