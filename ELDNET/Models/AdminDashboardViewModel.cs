namespace ELDNET.Models
{
    public class AdminDashboardViewModel
    {
        public int PendingGatePasses { get; set; }
        public int PendingLockerRequests { get; set; }
        public int PendingRoomReservations { get; set; }
        public int ChangedGatePasses { get; set; }
        public int ChangedLockerRequests { get; set; }
        public int ChangedRoomReservations { get; set; }
        public int TotalGatePasses { get; set; }
        public int TotalLockerRequests { get; set; }
        public int TotalRoomReservations { get; set; }
        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }
    }
}