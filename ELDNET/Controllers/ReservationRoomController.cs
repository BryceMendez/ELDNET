using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ELDNET.Controllers
{
    public class ReservationRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReservationRoom
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null) // not logged in
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Create");
        }

        // GET: ReservationRoom/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReservationRoom/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReservationRoom room)
        {
            if (ModelState.IsValid)
            {
                _context.ReservationRooms.Add(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: ReservationRoom/Details/5
        public IActionResult Details(int id)
        {
            var room = _context.ReservationRooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();
            return View(room);
        }

        // GET: ReservationRoom/Edit/5
        public IActionResult Edit(int id)
        {
            var room = _context.ReservationRooms.Find(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: ReservationRoom/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ReservationRoom room)
        {
            if (ModelState.IsValid)
            {
                _context.ReservationRooms.Update(room);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: ReservationRoom/Delete/5
        public IActionResult Delete(int id)
        {
            var room = _context.ReservationRooms.Find(id);
            if (room == null) return NotFound();
            return View(room);
        }

        // POST: ReservationRoom/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var room = _context.ReservationRooms.Find(id);
            if (room != null)
            {
                _context.ReservationRooms.Remove(room);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

