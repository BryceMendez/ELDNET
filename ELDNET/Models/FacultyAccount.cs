using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class FacultyAccount
    {
        public int Id { get; set; }

        [Required]
        public string? FacultyId { get; set; }  // e.g., fac-1001

        [Required]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }
    }
}
