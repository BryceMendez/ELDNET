using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;

namespace ELDNET.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminHomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to access the admin dashboard.";
                return RedirectToAction("Login", "Account");
            }

            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to access this page.";
                return RedirectToAction("Index", "Home");
            }

            // Fetch Pending Counts (as you currently have them)
            var pendingGatePass = await _context.GatePasses
                .CountAsync(g => g.Status == "Pending");
            var pendingLocker = await _context.LockerRequests
                .CountAsync(l => l.Status == "Pending");
            var pendingReservation = await _context.ReservationRooms
                .CountAsync(r => r.Status == "Pending");

            // Fetch Total Counts
            var totalGatePasses = await _context.GatePasses.CountAsync();
            var totalLockerRequests = await _context.LockerRequests.CountAsync();
            var totalRoomReservations = await _context.ReservationRooms.CountAsync();


            var totalStudents = await _context.StudentAccounts.CountAsync();
            var totalFaculty = await _context.FacultyAccounts.CountAsync();

            var dashboardViewModel = new AdminDashboardViewModel
            {
                PendingGatePasses = pendingGatePass,
                PendingLockerRequests = pendingLocker,
                PendingRoomReservations = pendingReservation,

                // Assign Total Counts
                TotalGatePasses = totalGatePasses,
                TotalLockerRequests = totalLockerRequests,
                TotalRoomReservations = totalRoomReservations,

                TotalStudents = totalStudents,
                TotalFaculty = totalFaculty
            };

            return View(dashboardViewModel);
        }
    }

    public class AdminDashboardViewModel
    {
        public int PendingGatePasses { get; set; }
        public int PendingLockerRequests { get; set; }
        public int PendingRoomReservations { get; set; }

        // New properties for total counts
        public int TotalGatePasses { get; set; }
        public int TotalLockerRequests { get; set; }
        public int TotalRoomReservations { get; set; }

        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }
    }
}