using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Linq; // Added for Where, FirstOrDefault, Any

namespace ELDNET.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Student - Admin only to view all student records
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to view student records.";
                return RedirectToAction("Login", "Account");
            }
            if (userRole != "Admin") // Only Admin can access this
            {
                TempData["error"] = "You are not authorized to view student records.";
                return RedirectToAction("Index", "Home");
            }

            IEnumerable<Student> objStudentList = _db.Students.ToList(); // Ensure it's evaluated for the view
            return View(objStudentList); // Return to a proper Index view
        }

        //GET: Student/Create - Admin only to create new student records
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to create student records.";
                return RedirectToAction("Login", "Account");
            }
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to create student records.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student obj)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null || userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to create student records.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _db.Students.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Student record created successfully";
                return RedirectToAction(nameof(Index)); // Redirect to the Index action of this controller
            }
            return View(obj);
        }

        //GET: Student/Edit/5 - Admin only to edit student records
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit student records.";
                return RedirectToAction("Login", "Account");
            }
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to edit student records.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0) return NotFound();

            var student = _db.Students.Find(id);
            if (student == null) return NotFound();

            return View(student);
        }

        //POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null || userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to edit student records.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _db.Students.Update(student);
                _db.SaveChanges();
                TempData["success"] = "Student record updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        //GET: Student/Details/5 - Admin only to view student record details
        [HttpGet]
        public IActionResult Details(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view student record details.";
                return RedirectToAction("Login", "Account");
            }
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to view student record details.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0) return NotFound();

            var student = _db.Students.Find(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // GET - Delete: Student/Delete/5 - Admin only to delete student records
        public IActionResult Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete student records.";
                return RedirectToAction("Login", "Account");
            }
            if (userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to delete student records.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0) return NotFound();

            var studentFromDb = _db.Students.Find(id);
            if (studentFromDb == null) return NotFound();

            return View(studentFromDb);
        }

        // POST - Delete: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null || userRole != "Admin")
            {
                TempData["error"] = "You are not authorized to delete student records.";
                return RedirectToAction("Index", "Home");
            }

            var student = _db.Students.Find(id);
            if (student == null) return NotFound();

            _db.Students.Remove(student);
            _db.SaveChanges();
            TempData["success"] = "Student record deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}