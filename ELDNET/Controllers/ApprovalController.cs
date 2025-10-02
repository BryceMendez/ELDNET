using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;

namespace ELDNET.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 Show all requests
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId"); // Get the UserId from session

            // 🔹 Check if not logged in
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to access the approval dashboard.";
                return RedirectToAction("Login", "Account");
            }

            // 🔹 Restrict Approval dashboard to Admin only
            // Assuming "admin" is the UserId for hardcoded admin, or the Username for Admin table.
            if (!(userRole == "Admin" && userId == "admin")) // Check both role and specific admin ID if needed
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home"); // Redirect to home for unauthorized users
            }

            var viewModel = new ApprovalViewModel
            {
                GatePasses = await _context.GatePasses.ToListAsync(),
                LockerRequests = await _context.LockerRequests.ToListAsync(),
                ActivityReservations = await _context.ReservationRooms.ToListAsync()
            };

            return View(viewModel);
        }

        // 🔹 GatePass actions
        [HttpPost]
        public async Task<IActionResult> ApproveGatePass(int id)
        {
            // Authorization check (same as Index, but for POST)
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var gp = await _context.GatePasses.FindAsync(id);
            if (gp != null)
            {
                gp.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["success"] = "Gate Pass approved successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyGatePass(int id)
        {
            // Authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var gp = await _context.GatePasses.FindAsync(id);
            if (gp != null)
            {
                gp.Status = "Denied";
                await _context.SaveChangesAsync();
                TempData["success"] = "Gate Pass denied.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Locker actions
        [HttpPost]
        public async Task<IActionResult> ApproveLocker(int id)
        {
            // Authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var locker = await _context.LockerRequests.FindAsync(id);
            if (locker != null)
            {
                locker.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["success"] = "Locker Request approved successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyLocker(int id)
        {
            // Authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var locker = await _context.LockerRequests.FindAsync(id);
            if (locker != null)
            {
                locker.Status = "Denied";
                await _context.SaveChangesAsync();
                TempData["success"] = "Locker Request denied.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Reservation actions
        [HttpPost]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            // Authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var res = await _context.ReservationRooms.FindAsync(id);
            if (res != null)
            {
                res.Status = "Approved";
                await _context.SaveChangesAsync();
                TempData["success"] = "Room Reservation approved successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyReservation(int id)
        {
            // Authorization check
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            var res = await _context.ReservationRooms.FindAsync(id);
            if (res != null)
            {
                res.Status = "Denied";
                await _context.SaveChangesAsync();
                TempData["success"] = "Room Reservation denied.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Details methods (also need authorization)
        public async Task<IActionResult> GatePassDetails(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home");
            }

            var gp = await _context.GatePasses.FindAsync(id);
            if (gp == null) return NotFound();
            return View(gp);
        }

        public async Task<IActionResult> Details(int id, string type)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home");
            }

            if (type == "GatePass")
            {
                var gp = await _context.GatePasses.FindAsync(id);
                if (gp == null) return NotFound();
                return View("GatePassDetails", gp);
            }
            else if (type == "Locker")
            {
                var locker = await _context.LockerRequests.FindAsync(id);
                if (locker == null) return NotFound();
                return View("LockerRequestDetails", locker);
            }
            else if (type == "Reservation")
            {
                var reservation = await _context.ReservationRooms.FindAsync(id);
                if (reservation == null) return NotFound();
                return View("ReservationDetails", reservation);
            }

            return NotFound();
        }
    }
}