using System;
using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class ReservationRoom
    {
        public int Id { get; set; }

        // 🔹 Link to Student
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

        // 🔹 Overall status
        public string? Status { get; set; } = "Pending";

        public DateTime Date { get; set; }

        public bool IsChangedByApplicant { get; set; } = false;

        // 🔹 Approval Workflow Fields (5 approvers)
        public string Approver1Name { get; set; } = "Bladimir Swarovski (OSD Director)";
        public string Approver1Status { get; set; } = "Pending";

        public string Approver2Name { get; set; } = "Maria Lopez (Dean of Students)";
        public string Approver2Status { get; set; } = "Pending";

        public string Approver3Name { get; set; } = "Antonio Reyes (Campus Administrator)";
        public string Approver3Status { get; set; } = "Pending";

        public string Approver4Name { get; set; } = "Clara Fernandez (Facilities Head)";
        public string Approver4Status { get; set; } = "Pending";

        public string Approver5Name { get; set; } = "David Johnson (Finance Officer)";
        public string Approver5Status { get; set; } = "Pending";

        public string FinalStatus { get; set; } = "Pending";
    }
}
