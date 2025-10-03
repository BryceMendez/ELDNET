using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        public int Id { get; set; }
        public string? StudentId { get; set; }

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

        [Display(Name = "Study Load")]
        public string? StudyLoadPath { get; set; }

        [Display(Name = "Registration Form")]
        public string? RegistrationPath { get; set; }

        [NotMapped]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        public IFormFile? RegistrationFile { get; set; }

        public bool IsChangedByApplicant { get; set; } = false;

        // 🔹 New Approval Workflow Properties
        public string Approver1Name { get; set; } = "Bladimir Swarovski (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "John Smith (Security Head)";
        public string Approver3Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";
    }
}
