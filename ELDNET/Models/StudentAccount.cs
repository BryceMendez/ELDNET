using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class StudentAccount
    {
        public int Id { get; set; }  // Primary key

        [Required]
        public string? StudentId { get; set; }  // Auto-generated like ucb-1234

        [Required]
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }  // Store hashed password
    }
}