namespace ELDNET.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string? UniqueId { get; set; } // StudentId or FacultyId
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; } // To indicate if it's a Student or Faculty
        public string? ProfilePictureUrl { get; set; } = "/images/default-profile.png";
    }
}