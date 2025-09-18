using System;
using System.ComponentModel.DataAnnotations;

namespace ELDNET.Models
{
    public class ReservationRoom
    {
        public int Id { get; set; }
        public string? OrganizationName { get; set; }
        public string? ActivityTitle { get; set; }
        public DateTime ActivityDate { get; set; }
        public string? Speaker { get; set; }
        public string? Venue { get; set; }
        public string? PurposeObjective { get; set; }
        public DateTime? DateNeeded { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? Participants { get; set; }
        public string? EquipmentFacilities { get; set; }
        public string? NatureOfActivity { get; set; }
        public string? SourceOfFunds { get; set; }
        public string? RoomNumber { get; set; }
        public string? ReservedBy { get; set; }

        // ✅ Add this
        public DateTime Date { get; set; }
    }

}
