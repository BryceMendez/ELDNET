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

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to access the approval dashboard.";
                return RedirectToAction("Login", "Account");
            }

            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new ApprovalViewModel
            {
                GatePasses = await _context.GatePasses.ToListAsync(),
                LockerRequests = await _context.LockerRequests.ToListAsync(),
                ActivityReservations = await _context.ReservationRooms.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SetApproval(int id, string type, int approver, string decision)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            object entity = await FindEntityAsync(type, id);
            if (entity == null) return NotFound();

            var statusProp = entity.GetType().GetProperty($"Approver{approver}Status");
            if (statusProp != null)
                statusProp.SetValue(entity, decision);

            UpdateFinalStatus(entity);
            await _context.SaveChangesAsync();

            TempData["success"] = $"Approver {approver} set to {decision}.";
            return RedirectToAction(nameof(Details), new { id, type });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmApproval(int id, string type)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to perform this action.";
                return RedirectToAction("Index", "Home");
            }

            object entity = await FindEntityAsync(type, id);
            if (entity == null) return NotFound();

            UpdateFinalStatus(entity);
            var finalProp = entity.GetType().GetProperty("FinalStatus");
            var statusProp = entity.GetType().GetProperty("Status");

            if (finalProp != null && statusProp != null)
            {
                var finalValue = finalProp.GetValue(entity)?.ToString();
                statusProp.SetValue(entity, finalValue);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Final decision has been confirmed.";
            return RedirectToAction(nameof(Index));
        }

        private void UpdateFinalStatus(object entity)
        {
            var s1 = entity.GetType().GetProperty("Approver1Status")?.GetValue(entity)?.ToString();
            var s2 = entity.GetType().GetProperty("Approver2Status")?.GetValue(entity)?.ToString();
            var s3 = entity.GetType().GetProperty("Approver3Status")?.GetValue(entity)?.ToString();
            var currentStatus = entity.GetType().GetProperty("Status")?.GetValue(entity)?.ToString(); // Get the current Status

            var final = "Pending";
            bool hasDecisions = !string.IsNullOrEmpty(s1) || !string.IsNullOrEmpty(s2) || !string.IsNullOrEmpty(s3);

            if (currentStatus == "Changed" && !hasDecisions)
            {
                final = "Changed";
            }
            else if (s1 == "Denied" || s2 == "Denied" || s3 == "Denied")
            {
                final = "Denied";
            }
            else if (s1 == "Approved" && s2 == "Approved" && s3 == "Approved")
            {
                final = "Approved";
            }
            // If it's not "Changed", not Denied, and not Fully Approved, it remains "Pending" (default)

            var finalProp = entity.GetType().GetProperty("FinalStatus");
            if (finalProp != null)
                finalProp.SetValue(entity, final);
        }

        private async Task<object?> FindEntityAsync(string type, int id)
        {
            return type switch
            {
                "GatePass" => await _context.GatePasses.FindAsync(id),
                "Locker" => await _context.LockerRequests.FindAsync(id),
                "Reservation" => await _context.ReservationRooms.FindAsync(id),
                _ => null
            };
        }

        public async Task<IActionResult> Details(int id, string type)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin")
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