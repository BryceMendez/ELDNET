using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using Microsoft.EntityFrameworkCore;

namespace ELDNET.Controllers
{
    public class FacultyAccountsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FacultyAccountsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: FacultyAccounts
        public IActionResult Index()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var facultyAccounts = _db.FacultyAccounts
                .OrderByDescending(f => f.Id)
                .ToList();

            return View(facultyAccounts);
        }

        // Optional: GET Details
        public IActionResult Details(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var faculty = _db.FacultyAccounts.Find(id);
            if (faculty == null)
            {
                return NotFound();
            }

            return View(faculty);
        }

        // Optional: POST Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var faculty = _db.FacultyAccounts.Find(id);
            if (faculty != null)
            {
                _db.FacultyAccounts.Remove(faculty);
                _db.SaveChanges();
                TempData["Message"] = "Faculty account deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}