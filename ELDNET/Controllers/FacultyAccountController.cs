using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ELDNET.Controllers
{
    public class FacultyAccountsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FacultyAccountsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
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

            if (string.IsNullOrEmpty(faculty.ProfilePictureUrl))
            {

            }

            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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