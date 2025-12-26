using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;
using System.Collections.Generic;
using System.Linq; 
using System;

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

            var pendingGatePasses = await _context.GatePasses
                .CountAsync(g => g.Status == "Pending");
            var pendingLockerRequests = await _context.LockerRequests
                .CountAsync(l => l.Status == "Pending");
            var pendingRoomReservations = await _context.ReservationRooms
                .CountAsync(r => r.Status == "Pending");
            var changedGatePasses = await _context.GatePasses
                .CountAsync(g => g.Status == "Changed");
            var changedLockerRequests = await _context.LockerRequests
                .CountAsync(l => l.Status == "Changed");
            var changedRoomReservations = await _context.ReservationRooms
                .CountAsync(r => r.Status == "Changed");

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
                ChangedGatePasses = changedGatePasses,
                ChangedLockerRequests = changedLockerRequests,
                ChangedRoomReservations = changedRoomReservations,
                TotalGatePasses = totalGatePasses,
                TotalLockerRequests = totalLockerRequests,
                TotalRoomReservations = totalRoomReservations,
                TotalStudents = totalStudents,
                TotalFaculty = totalFaculty
            };
            var modelList = new List<AdminDashboardViewModel> { dashboardViewModel };
            return View(modelList);
        }
    }
}