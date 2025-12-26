using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ELDNET.Models
{
    public class ProfileEditViewModel
    {
        public int Id { get; set; }
        public string? UniqueId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Course { get; set; }
        public string? Section { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public IFormFile ProfileImage { get; set; }
        public List<LockerRequest> LockerRequests { get; set; } = new List<LockerRequest>();
        public List<GatePass> GatePasses { get; set; } = new List<GatePass>();
        public List<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
    }
}