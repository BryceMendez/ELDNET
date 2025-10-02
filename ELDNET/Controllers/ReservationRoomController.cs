using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for AsNoTracking

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
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to view room reservations.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<ReservationRoom> reservationsQuery = _context.ReservationRooms;

            if (userRole == "Student")
            {
                // Students only see their own reservations
                reservationsQuery = reservationsQuery.Where(r => r.StudentId == studentId);
            }
            // Admins see all reservations

            var roomReservations = reservationsQuery.ToList();
            return View(roomReservations); // Return to a proper Index view
        }

        // GET: ReservationRoom/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin") // Only students can create
            {
                TempData["error"] = "Only students can create room reservations.";
                return RedirectToAction("Index", "Home");
            }

            var model = new ReservationRoom();
            if (fullName != null)
            {
                model.ReservedBy = fullName;
            }
            model.Date = DateTime.Now; // Pre-fill submission date
            model.ActivityDate = DateTime.Now; // Pre-fill activity date
            model.DateNeeded = DateTime.Now; // Pre-fill date needed

            return View(model);
        }

        // POST: ReservationRoom/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationRoom room)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students can create room reservations.";
                return RedirectToAction("Index", "Home");
            }

            room.StudentId = studentId;

            // Clear specific ModelState entries if you're auto-setting them
            ModelState.Remove(nameof(room.StudentId));
            // ModelState.Remove(nameof(room.ReservedBy)); // If pre-filled and should not be changed

            if (ModelState.IsValid)
            {
                room.Date = DateTime.Now; // Ensure submission date is set on creation
                _context.ReservationRooms.Add(room);
                _context.SaveChanges();
                TempData["success"] = "Room Reservation created successfully";
                return RedirectToAction(nameof(Index));
            }

            // Re-assign pre-filled values if ModelState is invalid
            var fullName = HttpContext.Session.GetString("FullName");
            if (fullName != null) room.ReservedBy = fullName;
            room.Date = DateTime.Now;
            if (room.ActivityDate == DateTime.MinValue) room.ActivityDate = DateTime.Now; // If not valid, default
            if (room.DateNeeded == null) room.DateNeeded = DateTime.Now; // If not valid, default


            return View(room);
        }

        // GET: ReservationRoom/Details/5
        public IActionResult Details(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view room reservation details.";
                return RedirectToAction("Login", "Account");
            }

            var room = _context.ReservationRooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && room.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to view this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // GET: ReservationRoom/Edit/5
        public IActionResult Edit(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = _context.ReservationRooms.Find(id);
            if (room == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && room.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // POST: ReservationRoom/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReservationRoom room)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit room reservations.";
                return RedirectToAction("Login", "Account");
            }

            // Retrieve original to preserve StudentId and prevent tampering
            var originalRoom = _context.ReservationRooms.AsNoTracking().FirstOrDefault(r => r.Id == room.Id);
            if (originalRoom == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && originalRoom.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            // Preserve StudentId and original Status
            room.StudentId = originalRoom.StudentId;
            room.Status = originalRoom.Status;
            room.Date = originalRoom.Date; // Preserve original submission date

            // Clear specific ModelState entries if you're auto-setting them
            ModelState.Remove(nameof(room.StudentId));
            ModelState.Remove(nameof(room.Status));
            ModelState.Remove(nameof(room.Date));
            // ModelState.Remove(nameof(room.ReservedBy));

            if (ModelState.IsValid)
            {
                _context.ReservationRooms.Update(room);
                _context.SaveChanges();
                TempData["success"] = "Room Reservation updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: ReservationRoom/Delete/5
        public IActionResult Delete(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = _context.ReservationRooms.Find(id);
            if (room == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && room.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // POST: ReservationRoom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete room reservations.";
                return RedirectToAction("Login", "Account");
            }

            var room = _context.ReservationRooms.Find(id);
            if (room == null)
            {
                return NotFound();
            }

            // Authorization check
            if (userRole == "Student" && room.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this room reservation.";
                return RedirectToAction(nameof(Index));
            }

            _context.ReservationRooms.Remove(room);
            _context.SaveChanges();
            TempData["success"] = "Room Reservation deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}