using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;
using System.Reflection; // Added for Type reflection

namespace ELDNET.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: /Approval/Index
        public async Task<IActionResult> Index(string? tab)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to access the approval dashboard.";
                return RedirectToAction("Login", "Account");
            }

            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home");
            }

            // Ensure all lists are initialized, even if empty
            var viewModel = new ApprovalViewModel
            {
                GatePasses = await _context.GatePasses.ToListAsync() ?? new List<GatePass>(),
                LockerRequests = await _context.LockerRequests.ToListAsync() ?? new List<LockerRequest>(),
                ActivityReservations = await _context.ReservationRooms.ToListAsync() ?? new List<ReservationRoom>()
            };

            // Ensure the tab parameter is correctly capitalized for consistency with your Index view's logic
            ViewBag.ActiveTab = tab?.ToLower() ?? "gatepass"; // Default tab is lowercase "gatepass"
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

            var entity = await FindEntityAsync(type, id);
            if (entity == null)
            {
                TempData["error"] = "The requested item was not found.";
                // When redirecting, include the 'type' to go back to the correct tab if possible
                return RedirectToAction(nameof(Index), new { tab = type?.ToLower() });
            }

            // Use reflection safely
            var statusProp = entity.GetType().GetProperty($"Approver{approver}Status");
            if (statusProp != null && statusProp.CanWrite)
            {
                statusProp.SetValue(entity, decision);
            }

            UpdateFinalStatus(entity);
            await _context.SaveChangesAsync();

            TempData["success"] = $"Approver {approver} set to {decision}.";
            // Stay on the details page after setting approval
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

            var entity = await FindEntityAsync(type, id);
            if (entity == null)
            {
                TempData["error"] = "The requested item was not found.";
                // Crucial: Pass the 'type' back as 'tab' parameter
                return RedirectToAction(nameof(Index), new { tab = type?.ToLower() });
            }

            UpdateFinalStatus(entity);

            var finalProp = entity.GetType().GetProperty("FinalStatus");
            var statusProp = entity.GetType().GetProperty("Status");

            if (finalProp != null && statusProp != null) // Removed null check on finalProp.GetValue(entity) here due to UpdateFinalStatus ensuring it's not null.
            {
                string finalStatusValue = finalProp.GetValue(entity)?.ToString() ?? "Pending"; // Ensure it's a string
                statusProp.SetValue(entity, finalStatusValue);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Final decision has been confirmed.";
            // Crucial: Pass the 'type' back as 'tab' parameter
            return RedirectToAction(nameof(Index), new { tab = type?.ToLower() });
        }

        // --- Corrected UpdateFinalStatus method from previous response ---
        private void UpdateFinalStatus(object entity)
        {
            var s1 = entity.GetType().GetProperty("Approver1Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s2 = entity.GetType().GetProperty("Approver2Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s3 = entity.GetType().GetProperty("Approver3Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s4 = entity.GetType().GetProperty("Approver4Status")?.GetValue(entity)?.ToString() ?? "Pending"; // For ReservationRoom
            var s5 = entity.GetType().GetProperty("Approver5Status")?.GetValue(entity)?.ToString() ?? "Pending"; // For ReservationRoom

            var currentStatus = entity.GetType().GetProperty("Status")?.GetValue(entity)?.ToString() ?? "Pending";

            var finalDecision = "Pending";

            bool isGatePass = entity is GatePass;
            bool isLockerRequest = entity is LockerRequest;
            bool isReservationRoom = entity is ReservationRoom;

            if (isLockerRequest)
            {
                if (s1 == "Denied") finalDecision = "Denied";
                else if (s1 == "Approved") finalDecision = "Approved";
            }
            else if (isGatePass)
            {
                if (s1 == "Denied" || s2 == "Denied" || s3 == "Denied") finalDecision = "Denied";
                else if (s1 == "Approved" && s2 == "Approved" && s3 == "Approved") finalDecision = "Approved";
            }
            else if (isReservationRoom)
            {
                if (s1 == "Denied" || s2 == "Denied" || s3 == "Denied" || s4 == "Denied" || s5 == "Denied") finalDecision = "Denied";
                else if (s1 == "Approved" && s2 == "Approved" && s3 == "Approved" && s4 == "Approved" && s5 == "Approved") finalDecision = "Approved";
            }

            // You might want to re-evaluate the "Changed" status logic here if needed.
            // For now, if an explicit decision (Approved/Denied) is reached, that takes precedence.
            // If it remains "Pending" AND currentStatus was "Changed", you could keep "Changed" or make it "Pending" for review.
            // Example:
            // if (finalDecision == "Pending" && currentStatus == "Changed")
            // {
            //     finalDecision = "Changed"; // Keep "Changed" if still pending review by approvers.
            // }


            var finalProp = entity.GetType().GetProperty("FinalStatus");
            if (finalProp != null && finalProp.CanWrite)
            {
                finalProp.SetValue(entity, finalDecision);
            }
        }
        // --- End of Corrected UpdateFinalStatus method ---

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

            // The 'type' parameter will become the 'tab' parameter when redirecting back to Index.
            // Store it in ViewBag to be accessible in the Details views for "Back to List" button.
            ViewBag.ActiveTabType = type?.ToLower();

            return type switch
            {
                "GatePass" => await _context.GatePasses.FindAsync(id) is GatePass gp ? View("GatePassDetails", gp) : NotFound(),
                "Locker" => await _context.LockerRequests.FindAsync(id) is LockerRequest locker ? View("LockerRequestDetails", locker) : NotFound(),
                "Reservation" => await _context.ReservationRooms.FindAsync(id) is ReservationRoom res ? View("ReservationDetails", res) : NotFound(),
                _ => NotFound()
            };
        }
    }
}