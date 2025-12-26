using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId { get; set; }
        public string? FacultyId { get; set; }
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
        [Display(Name = "Course & Year")]
        public string? CourseYear { get; set; }
        [Required]
        [Display(Name = "Plate Number")]
        public string? PlateNumber { get; set; }
        [Required]
        [Display(Name = "Vehicle Type")]
        public string? VehicleType { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Registration Expiry Date")]
        public DateTime RegistrationExpiryDate { get; set; }
        [Required]
        public string? Maker { get; set; }
        [Required]
        public string? Color { get; set; }
        public string Status { get; set; } = "Pending";
        public string? StudyLoadPath { get; set; }
        public string? RegistrationPath { get; set; }
        [NotMapped]
        public IFormFile? StudyLoadFile { get; set; }
        [NotMapped]
        public IFormFile? RegistrationFile { get; set; }
        public bool IsChangedByApplicant { get; set; } = false;
        public string Approver1Name { get; set; } = "Atty. Virgil B. Villanueva (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";
        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";
        public string Approver3Name { get; set; } = "John Smith (Security Head)";
        public string Approver3Status { get; set; } = "Pending";
        public string FinalStatus { get; set; } = "Pending";
        public string? DenialReason { get; set; }
    }
}