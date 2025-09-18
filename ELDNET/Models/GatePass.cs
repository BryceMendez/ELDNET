using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Models
{
    public class GatePass
    {
        public int Id { get; set; }

        [Required]
        public string? SchoolYear { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string? Role { get; set; }

        public string? Department { get; set; }

        public string? CourseYear { get; set; }

        public string? PlateNumber { get; set; }
        public string? Maker { get; set; }
        public string? Color { get; set; }

        // New columns for file paths
        public string? StudyLoadPath { get; set; }
        public string? RegistrationPath { get; set; }

        // Not mapped — only used for file upload
        [NotMapped]
        public IFormFile? StudyLoadFile { get; set; }

        [NotMapped]
        public IFormFile? RegistrationFile { get; set; }
    }
}
