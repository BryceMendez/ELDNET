using System.ComponentModel.DataAnnotations; // Add this if not present

namespace ELDNET.Models
{
    public class FacultyAccount
    {
        [Key] // Denotes this property as the primary key
        public int Id { get; set; }

        [Required] // Ensures this field is not null in the database
        public string FacultyId { get; set; } // e.g., "ucbf-123456"

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress] // Provides client-side and server-side validation for email format
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Stores the hashed password
    }
}