using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ELDNET.Data;
using ELDNET.Models;
using System.Reflection;

namespace ELDNET.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int MAX_CAR_SLOTS = 50;
        private const int MAX_MOTORCYCLE_SLOTS = 100;

        public ApprovalController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index(string? tab)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Login", "Account");

            var viewModel = new ApprovalViewModel
            {
                GatePasses = await _context.GatePasses.ToListAsync(),
                LockerRequests = await _context.LockerRequests.ToListAsync(),
                ActivityReservations = await _context.ReservationRooms.ToListAsync()
            };
            ViewBag.ActiveTab = tab?.ToLower() ?? "gatepass";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SetApproval(int id, string type, int approver, string decision)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return Unauthorized();

            var entity = await FindEntityAsync(type, id);
            if (entity == null) return NotFound();

            var statusProp = entity.GetType().GetProperty($"Approver{approver}Status");
            if (statusProp != null && statusProp.CanWrite) statusProp.SetValue(entity, decision);

            UpdateFinalStatus(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id, type });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmApproval(int id, string type, string? denialReason)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return Unauthorized();

            var entity = await FindEntityAsync(type, id);
            if (entity == null) return NotFound();

            UpdateFinalStatus(entity);

            var finalProp = entity.GetType().GetProperty("FinalStatus");
            var statusProp = entity.GetType().GetProperty("Status");
            string reasonPropertyName = entity switch
            {
                GatePass => "DenialReason",
                ReservationRoom => "DenialReasonReservation",
                LockerRequest => "DenialReasonLocker",
                _ => "DenialReason"
            };
            var reasonProp = entity.GetType().GetProperty(reasonPropertyName);

            if (finalProp != null && statusProp != null)
            {
                string calculatedValue = finalProp.GetValue(entity)?.ToString() ?? "Pending";

                if (entity is GatePass gp && calculatedValue == "Approved")
                {
                    int currentApprovedCount = await _context.GatePasses.CountAsync(x => x.VehicleType == gp.VehicleType && x.Status == "Approved" && x.Id != gp.Id);
                    int limit = gp.VehicleType == "Car" ? MAX_CAR_SLOTS : MAX_MOTORCYCLE_SLOTS;
                    if (currentApprovedCount >= limit)
                    {
                        TempData["error"] = $"Slots full for {gp.VehicleType}.";
                        return RedirectToAction(nameof(Details), new { id, type });
                    }
                }

                if (entity is ReservationRoom res && calculatedValue == "Approved")
                {
                    bool isAlreadyBooked = await _context.ReservationRooms
                        .AnyAsync(r => r.Id != res.Id &&
                                       r.RoomNumber == res.RoomNumber &&
                                       r.ActivityDate.Date == res.ActivityDate.Date &&
                                       (r.Status == "Approved" || r.FinalStatus == "Approved") &&
                                       res.TimeFrom < r.TimeTo &&
                                       res.TimeTo > r.TimeFrom);

                    if (isAlreadyBooked)
                    {
                        TempData["error"] = $"Conflict Detected: Room {res.RoomNumber} has already been approved for another request at this time. You cannot approve this request.";
                        return RedirectToAction(nameof(Details), new { id, type });
                    }
                }
                if (calculatedValue == "Denied")
                {
                    if (string.IsNullOrWhiteSpace(denialReason))
                    {
                        TempData["error"] = "A reason is required to deny this request.";
                        return RedirectToAction(nameof(Details), new { id, type });
                    }
                    reasonProp?.SetValue(entity, denialReason);
                }
                else
                {
                    reasonProp?.SetValue(entity, null);
                }

                statusProp.SetValue(entity, calculatedValue);
            }
            await _context.SaveChangesAsync();
            TempData["success"] = "Status updated successfully.";
            return RedirectToAction(nameof(Index), new { tab = type?.ToLower() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequest(int id, string type)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return Unauthorized();

            var entity = await FindEntityAsync(type, id);
            if (entity == null) return NotFound();
            if (entity is GatePass gp)
            {
                DeletePhysicalFile(gp.StudyLoadPath);
                DeletePhysicalFile(gp.RegistrationPath);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["success"] = "The record has been permanently deleted.";
            return RedirectToAction(nameof(Index), new { tab = type?.ToLower() });
        }
        public async Task<IActionResult> Details(int id, string type)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin") return RedirectToAction("Index", "Home");

            ViewBag.ActiveTabType = type?.ToLower();

            return type?.ToLower() switch
            {
                "gatepass" => await _context.GatePasses.FindAsync(id) is GatePass gp ? View("GatePassDetails", gp) : NotFound(),
                "locker" => await _context.LockerRequests.FindAsync(id) is LockerRequest locker ? View("LockerRequestDetails", locker) : NotFound(),
                "reservation" => await _context.ReservationRooms.FindAsync(id) is ReservationRoom res ? View("ReservationDetails", res) : NotFound(),
                _ => NotFound()
            };
        }

        private void UpdateFinalStatus(object entity)
        {
            var s1 = entity.GetType().GetProperty("Approver1Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s2 = entity.GetType().GetProperty("Approver2Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s3 = entity.GetType().GetProperty("Approver3Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s4 = entity.GetType().GetProperty("Approver4Status")?.GetValue(entity)?.ToString() ?? "Pending";
            var s5 = entity.GetType().GetProperty("Approver5Status")?.GetValue(entity)?.ToString() ?? "Pending";

            string calculatedDecision = "Pending";

            if (entity is GatePass)
            {
                if (s1 == "Denied" || s2 == "Denied" || s3 == "Denied") calculatedDecision = "Denied";
                else if (s1 == "Approved" && s2 == "Approved" && s3 == "Approved") calculatedDecision = "Approved";
            }
            else if (entity is LockerRequest)
            {
                if (s1 == "Denied") calculatedDecision = "Denied";
                else if (s1 == "Approved") calculatedDecision = "Approved";
            }
            else if (entity is ReservationRoom)
            {
                if (s1 == "Denied" || s2 == "Denied" || s3 == "Denied" || s4 == "Denied" || s5 == "Denied") calculatedDecision = "Denied";
                else if (s1 == "Approved" && s2 == "Approved" && s3 == "Approved" && s4 == "Approved" && s5 == "Approved") calculatedDecision = "Approved";
            }
            var finalProp = entity.GetType().GetProperty("FinalStatus");
            if (finalProp != null && finalProp.CanWrite) finalProp.SetValue(entity, calculatedDecision);
        }
        private async Task<object?> FindEntityAsync(string type, int id)
        {
            return type?.ToLower() switch
            {
                "gatepass" => await _context.GatePasses.FindAsync(id),
                "locker" => await _context.LockerRequests.FindAsync(id),
                "reservation" => await _context.ReservationRooms.FindAsync(id),
                _ => null
            };
        }
        private void DeletePhysicalFile(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }
    }
}