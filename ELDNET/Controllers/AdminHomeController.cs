using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;
using System.Collections.Generic; // Add this for List<T>
using System.Linq; // Add this for LINQ methods like Where, CountAsync
using System; // Add this for DateTime (though less critical with this approach)

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

            // Fetch Pending Counts (requests with Status == "Pending")
            var pendingGatePasses = await _context.GatePasses
                .CountAsync(g => g.Status == "Pending");
            var pendingLockerRequests = await _context.LockerRequests
                .CountAsync(l => l.Status == "Pending");
            var pendingRoomReservations = await _context.ReservationRooms
                .CountAsync(r => r.Status == "Pending");

            // Fetch Changed Counts (requests with Status == "Changed")
            // This directly leverages the "Changed" status you already have in your logic.
            var changedGatePasses = await _context.GatePasses
                .CountAsync(g => g.Status == "Changed");
            var changedLockerRequests = await _context.LockerRequests
                .CountAsync(l => l.Status == "Changed");
            var changedRoomReservations = await _context.ReservationRooms
                .CountAsync(r => r.Status == "Changed");


            // Fetch Total Counts
            var totalGatePasses = await _context.GatePasses.CountAsync();
            var totalLockerRequests = await _context.LockerRequests.CountAsync();
            var totalRoomReservations = await _context.ReservationRooms.CountAsync();


            var totalStudents = await _context.StudentAccounts.CountAsync();
            var totalFaculty = await _context.FacultyAccounts.CountAsync();

            var dashboardViewModel = new AdminDashboardViewModel
            {
                PendingGatePasses = pendingGatePasses,
                PendingLockerRequests = pendingLockerRequests,
                PendingRoomReservations = pendingRoomReservations,

                // Assign Changed Counts based on the "Changed" status
                ChangedGatePasses = changedGatePasses,
                ChangedLockerRequests = changedLockerRequests,
                ChangedRoomReservations = changedRoomReservations,

                // Assign Total Counts
                TotalGatePasses = totalGatePasses,
                TotalLockerRequests = totalLockerRequests,
                TotalRoomReservations = totalRoomReservations,

                TotalStudents = totalStudents,
                TotalFaculty = totalFaculty
            };

            // FIX: Wrap the single dashboardViewModel in a List to match the view's @model IEnumerable<T>
            var modelList = new List<AdminDashboardViewModel> { dashboardViewModel };

            return View(modelList);
        }
    }
}