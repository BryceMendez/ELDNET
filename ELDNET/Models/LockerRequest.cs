using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELDNET.Models
{
    public class LockerRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IdNumber { get; set; }
        public string? LockerNumber { get; set; }
        public string? Semester { get; set; }
        public string? ContactNumber { get; set; }

        // saved in DB
        public string? StudyLoadPath { get; set; }
        public string? RegistrationPath { get; set; }

        // not saved in DB, used for uploads
        [NotMapped]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        public IFormFile? RegistrationFile { get; set; }

        public bool AcceptTerms { get; set; }
    }
}
