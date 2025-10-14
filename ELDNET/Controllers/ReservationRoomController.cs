using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
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

        // GET: ReservationRoom - Displays list of reservations
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId"); // This will be StudentId or FacultyId

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to view room reservations.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<ReservationRoom> reservationsQuery = _context.ReservationRooms;

            if (userRole == "Student")
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "Student ID not found in session.";
                    return RedirectToAction("Login", "Account");
                }
                reservationsQuery = reservationsQuery.Where(r => r.StudentId == userId);
            }
            else if (userRole == "Faculty") // Add this condition for Faculty
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "Faculty ID not found in session.";
                    return RedirectToAction("Login", "Account");
                }
                reservationsQuery = reservationsQuery.Where(r => r.FacultyId == userId);
            }
            // If userRole is Admin, no filter is applied, they see all.

            var roomReservations = await reservationsQuery.OrderByDescending(r => r.Date).ToListAsync();

            return View(roomReservations);
        }

        // GET: ReservationRoom/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var userId = HttpContext.Session.GetString("UserId"); // Use userId consistently

            if (string.IsNullOrEmpty(userRole) || userRole == "Admin")
            {
                TempData["error"] = "Only students and faculty can create room reservations.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var model = new ReservationRoom
            {
                ActivityDate = DateTime.Today,
                DateNeeded = DateTime.Today,
                TimeFrom = new TimeSpan(8, 0, 0),
                TimeTo = new TimeSpan(9, 0, 0),
                Status = "Pending",
                FinalStatus = "Pending",
                Approver1Status = "Pending",
                Approver2Status = "Pending",
                Approver3Status = "Pending",
                ReservedBy = fullName, // Pre-fill from session
            };

            if (userRole == "Student")
            {
                model.StudentId = userId; // Set StudentId if the user is a student
            }
            else if (userRole == "Faculty")
            {
                model.FacultyId = userId; // Set FacultyId if the user is faculty
            }

            return View(model);
        }

        // POST: ReservationRoom/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationRoom room)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName");

            if (string.IsNullOrEmpty(userRole) || userRole == "Admin")
            {
                TempData["error"] = "Only students and faculty can create room reservations.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Manually set system-managed fields and fields from session
            if (userRole == "Student")
            {
                room.StudentId = userId;
                room.FacultyId = null; // Ensure FacultyId is null for students
            }
            else if (userRole == "Faculty")
            {
                room.FacultyId = userId;
                room.StudentId = null; // Ensure StudentId is null for faculty
            }

            room.Date = DateTime.Now;
            room.ReservedBy = fullName;

            room.Status = "Pending";
            room.FinalStatus = "Pending";
            room.Approver1Status = "Pending";
            room.Approver2Status = "Pending";
            room.Approver3Status = "Pending";

            // Remove ModelState entries for properties that are definitively set by the system
            // and should not be validated against user input.
            ModelState.Remove(nameof(room.StudentId));
            ModelState.Remove(nameof(room.FacultyId)); // Also remove FacultyId
            ModelState.Remove(nameof(room.Date));
            ModelState.Remove(nameof(room.Status));
            ModelState.Remove(nameof(room.FinalStatus));
            ModelState.Remove(nameof(room.Approver1Status));
            ModelState.Remove(nameof(room.Approver2Status));
            ModelState.Remove(nameof(room.Approver3Status));

            if (string.IsNullOrEmpty(room.ReservedBy))
            {
                room.ReservedBy = fullName;
            }

            if (ModelState.IsValid)
            {
                _context.ReservationRooms.Add(room);
                await _context.SaveChangesAsync();
                TempData["success"] = "Room Reservation created successfully.";
                return RedirectToAction(nameof(Index));
            }

            room.ReservedBy = fullName;

            return View(room);
        }

        // GET: ReservationRoom/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to view room reservation details.";
                return RedirectToAction("Login", "Account");
            }

            var room = await _context.ReservationRooms.FirstOrDefaultAsync(r => r.Id == id);
            if (room == null) return NotFound();

            if (userRole == "Student" && room.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to view this room reservation.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && room.FacultyId != userId) // Add faculty check
            {
                TempData["error"] = "You are not authorized to view this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // GET: ReservationRoom/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to edit room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null) return NotFound();

            if (userRole == "Student" && room.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && room.FacultyId != userId) // Add faculty check
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            if (room.FinalStatus == "Approved" || room.Status == "Approved")
            {
                TempData["warning"] = "Approved room reservations cannot be edited. You can view the details.";
                return RedirectToAction(nameof(Details), new { id = room.Id });
            }

            return View(room);
        }

        // POST: ReservationRoom/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservationRoom room)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to edit room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var originalRoom = await _context.ReservationRooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == room.Id);
            if (originalRoom == null) return NotFound();

            if (userRole == "Student" && originalRoom.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && originalRoom.FacultyId != userId) // Add faculty check
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            if (originalRoom.FinalStatus == "Approved" || originalRoom.Status == "Approved")
            {
                TempData["warning"] = "Approved room reservations cannot be edited.";
                return RedirectToAction(nameof(Details), new { id = room.Id });
            }

            // Preserve system-managed fields and statuses from the original record
            room.StudentId = originalRoom.StudentId; // Preserve original StudentId
            room.FacultyId = originalRoom.FacultyId; // Preserve original FacultyId
            room.Date = originalRoom.Date; // Preserve original creation date

            // Reset approval flow if the form was successfully submitted
            room.Status = "Changed";
            room.FinalStatus = "Changed";
            room.Approver1Status = "Pending";
            room.Approver2Status = "Pending";
            room.Approver3Status = "Pending";

            // Preserve approver names (assuming they are static or pulled from elsewhere)
            room.Approver1Name = originalRoom.Approver1Name;
            room.Approver2Name = originalRoom.Approver2Name;
            room.Approver3Name = originalRoom.Approver3Name;
            room.Approver4Name = originalRoom.Approver4Name;
            room.Approver5Name = originalRoom.Approver5Name;

            // Clear ModelState entries for properties that are definitively set by the system
            ModelState.Remove(nameof(room.StudentId));
            ModelState.Remove(nameof(room.FacultyId)); // Remove from model state
            ModelState.Remove(nameof(room.Date));
            ModelState.Remove(nameof(room.Status));
            ModelState.Remove(nameof(room.FinalStatus));
            ModelState.Remove(nameof(room.Approver1Status));
            ModelState.Remove(nameof(room.Approver2Status));
            ModelState.Remove(nameof(room.Approver3Status));
            ModelState.Remove(nameof(room.Approver4Status));
            ModelState.Remove(nameof(room.Approver5Status));
            ModelState.Remove(nameof(room.Approver1Name));
            ModelState.Remove(nameof(room.Approver2Name));
            ModelState.Remove(nameof(room.Approver3Name));
            ModelState.Remove(nameof(room.Approver4Name));
            ModelState.Remove(nameof(room.Approver5Name));


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Room Reservation updated successfully. Status changed to 'Changed' - requires re-approval.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ReservationRoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: ReservationRoom/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to delete room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null) return NotFound();

            if (userRole == "Student" && room.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && room.FacultyId != userId) // Add faculty check
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // POST: ReservationRoom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to delete room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = await _context.ReservationRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            if (userRole == "Student" && room.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && room.FacultyId != userId) // Add faculty check
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            _context.ReservationRooms.Remove(room);
            await _context.SaveChangesAsync();
            TempData["success"] = "Room Reservation deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ReservationRoomExists(int id)
        {
            return await _context.ReservationRooms.AnyAsync(e => e.Id == id);
        }
    }
}