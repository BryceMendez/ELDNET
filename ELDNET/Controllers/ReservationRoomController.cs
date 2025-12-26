using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ELDNET.Controllers
{
    public class ReservationRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole)) return RedirectToAction("Login", "Account");

            IQueryable<ReservationRoom> query = _context.ReservationRooms;

            if (userRole == "Student") query = query.Where(r => r.StudentId == userId);
            else if (userRole == "Faculty") query = query.Where(r => r.FacultyId == userId);

            return View(await query.OrderByDescending(r => r.Date).ToListAsync());
        }

        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole == "Admin") return RedirectToAction("Index");

            var model = new ReservationRoom
            {
                ActivityDate = DateTime.Today,
                DateNeeded = DateTime.Today,
                TimeFrom = new TimeSpan(8, 0, 0),
                TimeTo = new TimeSpan(9, 0, 0),
                ReservedBy = HttpContext.Session.GetString("FullName") ?? "",
                Status = "Pending",
                FinalStatus = "Pending"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationRoom room)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (room.ActivityDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Cannot reserve for past dates.");
                return View(room);
            }

            ModelState.Remove("StudentId"); ModelState.Remove("FacultyId");
            ModelState.Remove("Status"); ModelState.Remove("FinalStatus");

            if (ModelState.IsValid)
            {

                var conflict = await _context.ReservationRooms
                    .Where(r => r.RoomNumber == room.RoomNumber &&
                                r.ActivityDate.Date == room.ActivityDate.Date &&
                                r.Status != "Denied")
                    .FirstOrDefaultAsync(r => room.TimeFrom < r.TimeTo && r.TimeFrom < room.TimeTo);

                if (conflict != null)
                {
                    TempData["errorTitle"] = "Room Already Reserved";
                    TempData["errorMessage"] = $"<b>{room.RoomNumber}</b> is unavailable because it is already reserved by <b>{conflict.ReservedBy}</b> " +
                                               $"for <b>'{conflict.ActivityTitle}'</b> on this date from " +
                                               $"<b>{conflict.TimeFrom:hh\\:mm} to {conflict.TimeTo:hh\\:mm}</b>.";
                    return View(room);
                }

                if (userRole == "Student") room.StudentId = userId; else room.FacultyId = userId;
                room.Date = DateTime.Now;
                room.Status = "Pending";
                room.FinalStatus = "Pending";

                room.Approver1Status = room.Approver2Status = room.Approver3Status = room.Approver4Status = room.Approver5Status = "Pending";

                _context.ReservationRooms.Add(room);
                await _context.SaveChangesAsync();
                TempData["success"] = "Reservation filed successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null) return NotFound();

            if (room.Status == "Approved")
            {
                TempData["error"] = "Editing is disabled for approved reservations.";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservationRoom room)
        {
            if (id != room.Id) return NotFound();

            var currentData = await _context.ReservationRooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (currentData == null) return NotFound();

            if (currentData.Status == "Approved")
            {
                TempData["error"] = "This request is already approved and cannot be modified.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("StudentId"); ModelState.Remove("FacultyId");

            if (ModelState.IsValid)
            {

                var conflict = await _context.ReservationRooms
                    .Where(r => r.Id != id &&
                                r.RoomNumber == room.RoomNumber &&
                                r.ActivityDate.Date == room.ActivityDate.Date &&
                                r.Status != "Denied")
                    .FirstOrDefaultAsync(r => room.TimeFrom < r.TimeTo && r.TimeFrom < room.TimeTo);

                if (conflict != null)
                {
                    TempData["errorTitle"] = "Schedule Conflict";
                    TempData["errorMessage"] = $"Cannot update. <b>{room.RoomNumber}</b> is already reserved by <b>{conflict.ReservedBy}</b> " +
                                               $"for <b>'{conflict.ActivityTitle}'</b> during <b>{conflict.TimeFrom:hh\\:mm} - {conflict.TimeTo:hh\\:mm}</b>.";
                    return View(room);
                }

                try
                {
                    room.StudentId = currentData.StudentId;
                    room.FacultyId = currentData.FacultyId;
                    room.Date = currentData.Date;
                    room.Status = "Pending";
                    room.FinalStatus = "Pending";

                    room.Approver1Status = room.Approver2Status = room.Approver3Status = room.Approver4Status = room.Approver5Status = "Pending";

                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Changes saved and request reset to Pending.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReservationRooms.Any(e => e.Id == room.Id)) return NotFound();
                    throw;
                }
            }
            return View(room);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null) return NotFound();

            if (room.Status == "Approved")
            {
                TempData["error"] = "Approved reservations cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null) return NotFound();

            if (room.Status == "Approved")
            {
                TempData["error"] = "Cannot delete an approved reservation.";
                return RedirectToAction(nameof(Index));
            }
            _context.ReservationRooms.Remove(room);
            await _context.SaveChangesAsync();
            TempData["success"] = "Reservation deleted.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.ReservationRooms.FirstOrDefaultAsync(m => m.Id == id);
            return room == null ? NotFound() : View(room);
        }
    }
}