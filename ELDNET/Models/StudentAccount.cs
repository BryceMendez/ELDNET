using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class StudentAccount
    {
        public int Id { get; set; }
        [Required]
        public string? StudentId { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
        public string? Gender { get; set; }
        public string? Course { get; set; }
        public string? Section { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
