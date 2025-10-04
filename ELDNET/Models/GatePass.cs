using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        /*[Required]
        public int Id { get; set; }
        [Required]
        public string? StudentId { get; set; }

        [Required]
        [Display(Name = "School Year")]
        public string? SchoolYear { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        public string? Department { get; set; }

        [Required]
        [Display(Name = "Course Year")]
        public string? CourseYear { get; set; }

        [Required]

        [Display(Name = "Plate Number")]
        public string? PlateNumber { get; set; }
        [Required]
        public string? Maker { get; set; }
        [Required]
        public string? Color { get; set; }

        public string? Status { get; set; } = "Pending";
        [Required]

        [Display(Name = "Study Load")]
        public string? StudyLoadPath { get; set; }
        [Required]

        [Display(Name = "Registration Form")]
        public string? RegistrationPath { get; set; }
        [Required]

        [NotMapped]
        public IFormFile? StudyLoadFile { get; set; }
        [Required]

        [NotMapped]
        public IFormFile? RegistrationFile { get; set; }
        [Required]

        public bool IsChangedByApplicant { get; set; } = false;

        // 🔹 New Approval Workflow Properties
        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "John Smith (Security Head)";
        public string Approver3Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";*/


        public int Id { get; set; }
       
        public string? StudentId { get; set; }

        
        [Display(Name = "School Year")]
        public string? SchoolYear { get; set; }

        
        public string? Name { get; set; }
        
        public string? Address { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        
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
        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "John Smith (Security Head)";
        public string Approver3Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";
    }
}
