using System.ComponentModel.DataAnnotations;
using System;

namespace ELDNET.Models
{
    public class ReservationRoom
    {
        [Key]
        public int Id { get; set; }
        public string? StudentId { get; set; }
        public string? FacultyId { get; set; }
        [Required(ErrorMessage = "Organization Name is required.")]
        [Display(Name = "Organization Name")]
        public string? OrganizationName { get; set; }
        [Required(ErrorMessage = "Activity Title is required.")]
        [Display(Name = "Activity Title")]
        public string? ActivityTitle { get; set; }
        public string? Speaker { get; set; }
        [Required(ErrorMessage = "Venue is required.")]
        public string? Venue { get; set; }
        [Required(ErrorMessage = "Room Number is required.")]
        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }
        [Required(ErrorMessage = "Purpose/Objective is required.")]
        [Display(Name = "Purpose / Objective")]
        public string? PurposeObjective { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Activity Date is required.")]
        [DataType(DataType.Date)]
        public DateTime ActivityDate { get; set; }
        [Required(ErrorMessage = "Date Needed is required.")]
        [DataType(DataType.Date)]
        public DateTime DateNeeded { get; set; }
        [Required(ErrorMessage = "Start Time is required.")]
        public TimeSpan TimeFrom { get; set; }
        [Required(ErrorMessage = "End Time is required.")]
        public TimeSpan TimeTo { get; set; }
        [Required(ErrorMessage = "Participants description is required.")]
        public string? Participants { get; set; }
        [Required(ErrorMessage = "Source of Funds is required.")]
        [Display(Name = "Source of Funds")]
        public string? SourceOfFunds { get; set; }
        [Display(Name = "Equipment and Facilities")]
        public string? EquipmentFacilities { get; set; }
        [Required(ErrorMessage = "Nature of Activity is required.")]
        [Display(Name = "Nature of Activity")]
        public string? NatureOfActivity { get; set; }
        [Required(ErrorMessage = "Reserved By is required.")]
        [Display(Name = "Reserved By")]
        public string? ReservedBy { get; set; }
        public string Status { get; set; } = "Pending";
        public string FinalStatus { get; set; } = "Pending";
        public string Approver1Name { get; set; } = "Mr. John Doe (Department Head)";
        public string Approver1Status { get; set; } = "Pending";
        public string Approver2Name { get; set; } = "Ms. Jane Smith (Dean)";
        public string Approver2Status { get; set; } = "Pending";
        public string Approver3Name { get; set; } = "Dr. Bill Gates (VP Academics)";
        public string Approver3Status { get; set; } = "Pending";
        public string Approver4Name { get; set; } = "Procurement Officer";
        public string Approver4Status { get; set; } = "Pending";
        public string Approver5Name { get; set; } = "Physical Plant Director";
        public string Approver5Status { get; set; } = "Pending";
        public string? DenialReasonReservation { get; set; }
    }
}