namespace ELDNET.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? ProfilePictureUrl { get; set; } = "/images/default-profile.png";
    }
}