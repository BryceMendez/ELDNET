using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(25)]

        public string Lastname { get; set; } = "";
        [Required, MaxLength(25)]
        public string? Firstname { get; set; }
        public string? Course { get; set; }
        public string? Email { get; set; }
        [Display(Name = "Date Created")]

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string? RequestType { get; set; }

        public string? RequestStatus { get; set; }

    }
}
