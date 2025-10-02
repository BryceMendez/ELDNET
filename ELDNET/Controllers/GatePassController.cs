using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for AsNoTracking

namespace ELDNET.Controllers
{
    public class GatePassController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GatePassController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX - Displays list of gate passes
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to view gate passes.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<GatePass> gatePassesQuery = _db.GatePasses;

            if (userRole == "Student")
            {
                // Students only see their own gate passes
                gatePassesQuery = gatePassesQuery.Where(gp => gp.StudentId == studentId);
            }
            // Admins see all gate passes

            var gatePassList = gatePassesQuery.ToList();
            return View(gatePassList); // Return to a proper Index view
        }

        // GET: CREATE
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");

            if (userRole == null || userRole == "Admin") // Only students can create
            {
                TempData["error"] = "Only students can create gate passes.";
                return RedirectToAction("Index", "Home");
            }

            var model = new GatePass();
            if (fullName != null)
            {
                model.Name = fullName;
            }
            model.Date = DateTime.Now; // Pre-fill date

            return View(model);
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GatePass obj)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students can create gate passes.";
                return RedirectToAction("Index", "Home");
            }

            obj.StudentId = studentId;

            // Clear specific ModelState entries if you're auto-setting them
            ModelState.Remove(nameof(obj.StudentId));
            // ModelState.Remove(nameof(obj.Name)); // If Name is pre-filled and should not be changed by user

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string studentUploadsFolder = Path.Combine(uploadsFolder, "gatepass", studentId ?? "unknown"); // Organize by student ID

                if (!Directory.Exists(studentUploadsFolder))
                    Directory.CreateDirectory(studentUploadsFolder);

                // Save Study Load file
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{obj.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(studentUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.StudyLoadFile.CopyTo(stream);
                    }
                    obj.StudyLoadPath = $"/uploads/gatepass/{studentId}/{uniqueFileName}";
                }

                // Save Registration Form file
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{obj.RegistrationFile.FileName}";
                    string filePath = Path.Combine(studentUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.RegistrationFile.CopyTo(stream);
                    }
                    obj.RegistrationPath = $"/uploads/gatepass/{studentId}/{uniqueFileName}";
                }

                _db.GatePasses.Add(obj);
                _db.SaveChanges();

                TempData["success"] = "Gate Pass created successfully";
                return RedirectToAction(nameof(Index));
            }

            // Re-assign pre-filled values if ModelState is invalid
            var fullName = HttpContext.Session.GetString("FullName");
            if (fullName != null) obj.Name = fullName;
            obj.Date = DateTime.Now; // Re-set date

            return View(obj);
        }

        // GET: EDIT
        public IActionResult Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && gatePass.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            return View(gatePass);
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GatePass obj)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (obj.Id == 0) return NotFound(); // Ensure Id is present for update

            // Retrieve original to preserve StudentId and prevent tampering
            var originalGatePass = _db.GatePasses.AsNoTracking().FirstOrDefault(gp => gp.Id == obj.Id);
            if (originalGatePass == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && originalGatePass.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            // Preserve StudentId and potentially other fields
            obj.StudentId = originalGatePass.StudentId;
            // Also preserve status if admin is the only one who should change it
            obj.Status = originalGatePass.Status;


            // Clear specific ModelState entries if you're auto-setting them
            ModelState.Remove(nameof(obj.StudentId));
            ModelState.Remove(nameof(obj.Status));
            // ModelState.Remove(nameof(obj.Name));

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string studentUploadsFolder = Path.Combine(uploadsFolder, "gatepass", originalGatePass.StudentId ?? "unknown");

                if (!Directory.Exists(studentUploadsFolder))
                    Directory.CreateDirectory(studentUploadsFolder);

                // Update Study Load file if uploaded
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{obj.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(studentUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.StudyLoadFile.CopyTo(stream);
                    }
                    obj.StudyLoadPath = $"/uploads/gatepass/{originalGatePass.StudentId}/{uniqueFileName}";
                }
                else
                {
                    // Preserve existing path if no new file uploaded
                    obj.StudyLoadPath = originalGatePass.StudyLoadPath;
                }

                // Update Registration Form file if uploaded
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{obj.RegistrationFile.FileName}";
                    string filePath = Path.Combine(studentUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.RegistrationFile.CopyTo(stream);
                    }
                    obj.RegistrationPath = $"/uploads/gatepass/{originalGatePass.StudentId}/{uniqueFileName}";
                }
                else
                {
                    // Preserve existing path if no new file uploaded
                    obj.RegistrationPath = originalGatePass.RegistrationPath;
                }

                _db.GatePasses.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Gate Pass updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // GET: DETAILS
        public IActionResult Details(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view gate pass details.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && gatePass.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to view this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            return View(gatePass);
        }

        // GET: DELETE
        public IActionResult Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && gatePass.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            return View(gatePass);
        }

        // POST: DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete gate passes.";
                return RedirectToAction("Login", "Account");
            }

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && gatePass.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            // Optional: Delete the files from wwwroot if they exist
            if (!string.IsNullOrEmpty(gatePass.StudyLoadPath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", gatePass.StudyLoadPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            if (!string.IsNullOrEmpty(gatePass.RegistrationPath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", gatePass.RegistrationPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }


            _db.GatePasses.Remove(gatePass);
            _db.SaveChanges();
            TempData["success"] = "Gate Pass deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}