using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Activity Title is required.")]
        [Display(Name = "Activity Title")]
        public string ActivityTitle { get; set; }

        [Display(Name = "Speaker")]
        public string? Speaker { get; set; } // Speaker might be optional

        [Required(ErrorMessage = "Venue is required.")]
        public string Venue { get; set; }

        // Added RoomNumber as it's required in your controller logic and validation
        [Required(ErrorMessage = "Room Number is required.")]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }

        [Required(ErrorMessage = "Purpose/Objective is required.")]
        [Display(Name = "Purpose / Objective")]
        public string PurposeObjective { get; set; }

        // Date of Submission - Set by the system, not by the user via form.
        [Required]
        [Display(Name = "Date Submitted")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Date of the actual activity - User provides this via form.
        [Required(ErrorMessage = "Activity Date is required.")]
        [Display(Name = "Activity Date")]
        [DataType(DataType.Date)]
        public DateTime ActivityDate { get; set; }

        // Date needed for setup/preparation - User provides this via form.
        [Required(ErrorMessage = "Date Needed is required.")]
        [Display(Name = "Date Needed (for setup/preparation)")]
        [DataType(DataType.Date)]
        public DateTime DateNeeded { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [Display(Name = "Time From")]
        [DataType(DataType.Time)]
        public TimeSpan TimeFrom { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        [Display(Name = "Time To")]
        [DataType(DataType.Time)]
        public TimeSpan TimeTo { get; set; }

        [Required(ErrorMessage = "Number of Participants is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of Participants must be at least 1.")]
        public int Participants { get; set; }

        [Required(ErrorMessage = "Source of Funds is required.")]
        [Display(Name = "Source of Funds")]
        public string SourceOfFunds { get; set; }

        [Display(Name = "Equipment and Facilities")]
        public string? EquipmentFacilities { get; set; } // Equipment might be optional

        [Required(ErrorMessage = "Nature of Activity is required.")]
        [Display(Name = "Nature of Activity")]
        public string NatureOfActivity { get; set; }

        // Reserved By - Pre-filled from session, but can be displayed.
        // It's [Required] so if session is null, validation will catch it.
        [Required(ErrorMessage = "Reserved By is required.")]
        [Display(Name = "Reserved By")]
        public string ReservedBy { get; set; }

        // Approval Statuses
        public string Status { get; set; } = "Pending";
        public string FinalStatus { get; set; } = "Pending";

        public string Approver1Name { get; set; } = "Mr. John Doe (Department Head)"; // Example
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Ms. Jane Smith (Dean)"; // Example
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "Dr. Bill Gates (VP Academics)"; // Example
        public string Approver3Status { get; set; } = "Pending";

        public string? Approver4Name { get; set; }
        public string? Approver4Status { get; set; }

        public string? Approver5Name { get; set; }
        public string? Approver5Status { get; set; }
    }
}