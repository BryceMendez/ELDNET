using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ELDNET.Models
{
    public class ApprovalViewModel
    {
        public IEnumerable<GatePass> GatePasses { get; set; } = new List<GatePass>();
        public IEnumerable<ReservationRoom> ActivityReservations { get; set; } = new List<ReservationRoom>();
        public IEnumerable<LockerRequest> LockerRequests { get; set; } = new List<LockerRequest>();
    }
}
