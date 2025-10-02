using System;
using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class ReservationRoom
    {
        public int Id { get; set; }

        // Add this property to link to the student
        public string? StudentId { get; set; } // Foreign key to StudentAccount.StudentId

        [Display(Name = "Organization Name")]
        public string? OrganizationName { get; set; }
        [Display(Name = "Activity Title")]
        public string? ActivityTitle { get; set; }
        [Display(Name = "Activity Date")]
        public DateTime ActivityDate { get; set; }
        public string? Speaker { get; set; }
        public string? Venue { get; set; }
        [Display(Name = "Purpose/Objective")]
        public string? PurposeObjective { get; set; }
        [Display(Name = "Date Needed")]
        public DateTime? DateNeeded { get; set; }
        [Display(Name = "Time From")]
        public TimeSpan? TimeFrom { get; set; }
        [Display(Name = "Time To")]
        public TimeSpan? TimeTo { get; set; }
        public string? Participants { get; set; }
        [Display(Name = "Equipment/Facilities Needed")]
        public string? EquipmentFacilities { get; set; }
        [Display(Name = "Nature of Activity")]
        public string? NatureOfActivity { get; set; }
        [Display(Name = "Source of Funds")]
        public string? SourceOfFunds { get; set; }
        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }
        [Display(Name = "Reserved By")]
        public string? ReservedBy { get; set; }
        public string? Status { get; set; } = "Pending";

        public DateTime Date { get; set; }
    }
}