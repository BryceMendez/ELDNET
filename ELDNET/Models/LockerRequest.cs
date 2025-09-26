using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELDNET.Models
{
    public class LockerRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Display(Name = "ID Number")]
        public string? IdNumber { get; set; }
        [Display(Name = "Locker Number")]
        public string? LockerNumber { get; set; }
        public string? Semester { get; set; }
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }
        public string? Status { get; set; } = "Pending";

        // saved in DB
        public string? StudyLoadPath { get; set; }
        public string? RegistrationPath { get; set; }

        // not saved in DB, used for uploads
        [NotMapped]
        [Display(Name = "Study Load")]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        [Display(Name = "Registration Form")]
        public IFormFile? RegistrationFile { get; set; }

        [Display(Name = "Accept Terms and Conditions")]
        public bool AcceptTerms { get; set; }
    }
}
