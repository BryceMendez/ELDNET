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
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userPassword = HttpContext.Session.GetString("UserPassword");

            // 🔹 Check if not logged in
            if (userRole == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 🔹 Restrict Approval dashboard to Admin only
            if (!(userEmail == "admin" && userPassword == "Admin@001"))
            {
                return Forbid(); // or RedirectToAction("AccessDenied", "Account")
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
            var gp = await _context.GatePasses.FindAsync(id);
            if (gp != null)
            {
                gp.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyGatePass(int id)
        {
            var gp = await _context.GatePasses.FindAsync(id);
            if (gp != null)
            {
                gp.Status = "Denied";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Locker actions
        [HttpPost]
        public async Task<IActionResult> ApproveLocker(int id)
        {
            var locker = await _context.LockerRequests.FindAsync(id);
            if (locker != null)
            {
                locker.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyLocker(int id)
        {
            var locker = await _context.LockerRequests.FindAsync(id);
            if (locker != null)
            {
                locker.Status = "Denied";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Reservation actions
        [HttpPost]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var res = await _context.ReservationRooms.FindAsync(id);
            if (res != null)
            {
                res.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DenyReservation(int id)
        {
            var res = await _context.ReservationRooms.FindAsync(id);
            if (res != null)
            {
                res.Status = "Denied";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GatePassDetails(int id)
        {
            var gp = await _context.GatePasses.FindAsync(id);
            if (gp == null) return NotFound();
            return View(gp);
        }

        public async Task<IActionResult> Details(int id, string type)
        {
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
