using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELDNET.Models
{
    public class LockerRequest
    {
        public int Id { get; set; }

        // 🔹 Link to Student
        public string? StudentId { get; set; } // Foreign key to StudentAccount.StudentId

        public string? Name { get; set; }

        [Display(Name = "ID Number")]
        public string? IdNumber { get; set; }

        [Display(Name = "Locker Number")]
        public string? LockerNumber { get; set; }

        public string? Semester { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        // 🔹 Overall Status
        public string? Status { get; set; } = "Pending";

        // 🔹 File paths (saved in DB)
        public string? StudyLoadPath { get; set; }
        public string? RegistrationPath { get; set; }

        // 🔹 Not saved in DB, used for uploads
        [NotMapped]
        [Display(Name = "Study Load")]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        [Display(Name = "Registration Form")]
        public IFormFile? RegistrationFile { get; set; }

        [Display(Name = "Accept Terms and Conditions")]
        public bool AcceptTerms { get; set; }

        public bool IsChangedByApplicant { get; set; } = false;

        // 🔹 Approval Workflow Fields
        public string Approver1Name { get; set; } = "Bladimir Swarovski (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";
    }
}
