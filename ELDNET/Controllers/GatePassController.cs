using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // For HttpContext.Session
using System.IO; // For Path, Directory, FileStream
using System; // For DateTime
using System.Threading.Tasks; // For async operations

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
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to view gate passes.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<GatePass> gatePassesQuery = _db.GatePasses;

            // --- CORRECTION FOR INDEX FILTERING START ---
            if (userRole == "Student")
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "Student ID not found in session.";
                    return RedirectToAction("Login", "Account");
                }
                gatePassesQuery = gatePassesQuery.Where(gp => gp.StudentId == userId);
            }
            else if (userRole == "Faculty") // Filter for Faculty's own gate passes
            {
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "Faculty ID not found in session.";
                    return RedirectToAction("Login", "Account");
                }
                gatePassesQuery = gatePassesQuery.Where(gp => gp.FacultyId == userId);
            }
            // Admins will see all gate passes as no filter is applied for them.
            // --- CORRECTION FOR INDEX FILTERING END ---

            var gatePassList = await gatePassesQuery.OrderByDescending(gp => gp.Date).ToListAsync();

            // Pass user role to the view to conditionally show the Create button
            ViewBag.UserRole = userRole;

            return View(gatePassList);
        }

        // GET: CREATE
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole) || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create gate passes.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var model = new GatePass
            {
                Name = fullName,
                Date = DateTime.Now,
                Status = "Pending",
                FinalStatus = "Pending",
                Approver1Status = "Pending",
                Approver2Status = "Pending",
                Approver3Status = "Pending"
            };

            // --- THIS LOGIC IS CORRECT: Sets StudentId or FacultyId based on role ---
            if (userRole == "Student")
            {
                model.StudentId = userId;
                model.FacultyId = null; // Ensure FacultyId is null for students
            }
            else if (userRole == "Faculty")
            {
                model.FacultyId = userId;
                model.StudentId = null; // Ensure StudentId is null for faculty
            }
            // --- END OF CORRECT LOGIC ---

            return View(model);
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GatePass obj)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName");

            if (string.IsNullOrEmpty(userRole) || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create gate passes.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // --- THIS LOGIC IS CORRECT: Re-assigns StudentId or FacultyId before saving ---
            // This is crucial because StudentId/FacultyId are not directly in the form inputs.
            if (userRole == "Student")
            {
                obj.StudentId = userId;
                obj.FacultyId = null;
            }
            else if (userRole == "Faculty")
            {
                obj.FacultyId = userId;
                obj.StudentId = null;
            }
            // --- END OF CORRECT LOGIC ---

            obj.Name = fullName;
            obj.Date = DateTime.Now;

            obj.Status = "Pending";
            obj.FinalStatus = "Pending";
            obj.Approver1Status = "Pending";
            obj.Approver2Status = "Pending";
            obj.Approver3Status = "Pending";

            // --- CHANGE HERE: No need to Remove ModelState for StudentId/FacultyId if they are nullable ---
            // However, removing them is harmless if you're populating them manually.
            // I'll keep them removed here as it doesn't hurt, and you might have other reasons.
            ModelState.Remove(nameof(obj.StudentId));
            ModelState.Remove(nameof(obj.FacultyId));
            ModelState.Remove(nameof(obj.Name));
            ModelState.Remove(nameof(obj.Date));
            ModelState.Remove(nameof(obj.Status));
            ModelState.Remove(nameof(obj.FinalStatus));
            ModelState.Remove(nameof(obj.Approver1Status));
            ModelState.Remove(nameof(obj.Approver2Status));
            ModelState.Remove(nameof(obj.Approver3Status));


            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                // Use the correct ID for the folder based on role
                string userSpecificId = userRole == "Student" ? obj.StudentId : obj.FacultyId;
                string userUploadsFolder = Path.Combine(uploadsFolder, "gatepass", userSpecificId ?? "unknown");


                if (!Directory.Exists(userUploadsFolder))
                    Directory.CreateDirectory(userUploadsFolder);

                // Save Study Load file
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{obj.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(userUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await obj.StudyLoadFile.CopyToAsync(stream);
                    }
                    obj.StudyLoadPath = $"/uploads/gatepass/{userSpecificId}/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(obj.StudyLoadFile), "Please upload the Study Load file.");
                    return View(obj);
                }


                // Save Registration Form file
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{obj.RegistrationFile.FileName}";
                    string filePath = Path.Combine(userUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await obj.RegistrationFile.CopyToAsync(stream);
                    }
                    obj.RegistrationPath = $"/uploads/gatepass/{userSpecificId}/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(obj.RegistrationFile), "Please upload the Registration Form file.");
                    return View(obj);
                }

                _db.GatePasses.Add(obj);
                await _db.SaveChangesAsync();

                TempData["success"] = "Gate Pass created successfully";
                return RedirectToAction(nameof(Index));
            }

            // --- THIS PART IS FINE, ensures model state for view is correct if validation fails ---
            obj.Name = fullName;
            if (userRole == "Student")
            {
                obj.StudentId = userId;
                obj.FacultyId = null;
            }
            else if (userRole == "Faculty")
            {
                obj.FacultyId = userId;
                obj.StudentId = null;
            }
            obj.Date = DateTime.Now;
            // --- END OF FINE PART ---

            return View(obj);
        }

        // GET: EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to edit gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();

            // --- THIS AUTHORIZATION LOGIC IS CORRECT ---
            if (userRole == "Student" && gatePass.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to edit this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && gatePass.FacultyId != userId)
            {
                TempData["error"] = "You are not authorized to edit this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            // --- END OF CORRECT AUTHORIZATION LOGIC ---

            if (gatePass.FinalStatus == "Approved" || gatePass.Status == "Approved")
            {
                TempData["info"] = "Approved gate passes cannot be edited. Viewing details instead.";
                return RedirectToAction(nameof(Details), new { id = gatePass.Id });
            }

            return View(gatePass);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GatePass obj)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to edit gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (obj.Id == 0) return NotFound();

            var originalGatePass = await _db.GatePasses.AsNoTracking().FirstOrDefaultAsync(gp => gp.Id == obj.Id);
            if (originalGatePass == null) return NotFound();

            // Authorization
            if ((userRole == "Student" && originalGatePass.StudentId != userId) ||
                (userRole == "Faculty" && originalGatePass.FacultyId != userId))
            {
                TempData["error"] = "You are not authorized to edit this gate pass.";
                return RedirectToAction(nameof(Index));
            }

            if (originalGatePass.FinalStatus == "Approved" || originalGatePass.Status == "Approved")
            {
                TempData["info"] = "Approved gate passes cannot be edited.";
                return RedirectToAction(nameof(Details), new { id = obj.Id });
            }

            // Preserve non-editable fields
            obj.StudentId = originalGatePass.StudentId;
            obj.FacultyId = originalGatePass.FacultyId;
            obj.Name = originalGatePass.Name;
            obj.Date = originalGatePass.Date;

            obj.Status = "Changed";
            obj.FinalStatus = "Changed";
            obj.Approver1Status = "Pending";
            obj.Approver2Status = "Pending";
            obj.Approver3Status = "Pending";
            obj.Approver1Name = originalGatePass.Approver1Name;
            obj.Approver2Name = originalGatePass.Approver2Name;
            obj.Approver3Name = originalGatePass.Approver3Name;

            // ✅ Skip validation for file inputs if no new files uploaded
            if (obj.StudyLoadFile == null)
                ModelState.Remove(nameof(obj.StudyLoadFile));
            if (obj.RegistrationFile == null)
                ModelState.Remove(nameof(obj.RegistrationFile));

            // Remove automatically set fields
            ModelState.Remove(nameof(obj.StudentId));
            ModelState.Remove(nameof(obj.FacultyId));
            ModelState.Remove(nameof(obj.Name));
            ModelState.Remove(nameof(obj.Date));
            ModelState.Remove(nameof(obj.Status));
            ModelState.Remove(nameof(obj.FinalStatus));
            ModelState.Remove(nameof(obj.Approver1Status));
            ModelState.Remove(nameof(obj.Approver2Status));
            ModelState.Remove(nameof(obj.Approver3Status));
            ModelState.Remove(nameof(obj.Approver1Name));
            ModelState.Remove(nameof(obj.Approver2Name));
            ModelState.Remove(nameof(obj.Approver3Name));

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string userSpecificId = userRole == "Student" ? originalGatePass.StudentId : originalGatePass.FacultyId;
                string userUploadsFolder = Path.Combine(uploadsFolder, "gatepass", userSpecificId ?? "unknown");
                Directory.CreateDirectory(userUploadsFolder);

                // ✅ Study Load file logic (keep old if not reuploaded)
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{obj.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(userUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await obj.StudyLoadFile.CopyToAsync(stream);
                    obj.StudyLoadPath = $"/uploads/gatepass/{userSpecificId}/{uniqueFileName}";
                }
                else
                {
                    obj.StudyLoadPath = originalGatePass.StudyLoadPath;
                }

                // ✅ Registration Form file logic (keep old if not reuploaded)
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{obj.RegistrationFile.FileName}";
                    string filePath = Path.Combine(userUploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await obj.RegistrationFile.CopyToAsync(stream);
                    obj.RegistrationPath = $"/uploads/gatepass/{userSpecificId}/{uniqueFileName}";
                }
                else
                {
                    obj.RegistrationPath = originalGatePass.RegistrationPath;
                }

                // ✅ Save
                _db.GatePasses.Update(obj);
                await _db.SaveChangesAsync();

                TempData["success"] = "Gate Pass updated successfully. Status changed to 'Changed' for re-approval.";
                return RedirectToAction(nameof(Index));
            }

            // Repopulate user info if validation fails
            obj.Name = HttpContext.Session.GetString("FullName");
            obj.StudentId = originalGatePass.StudentId;
            obj.FacultyId = originalGatePass.FacultyId;
            obj.Date = originalGatePass.Date;

            return View(obj);
        }


        // GET: DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to view gate pass details.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();

            // --- CORRECTION FOR DETAILS AUTHORIZATION START ---
            if (userRole == "Student" && gatePass.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to view this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && gatePass.FacultyId != userId) // Faculty can only view their own
            {
                TempData["error"] = "You are not authorized to view this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            // --- CORRECTION FOR DETAILS AUTHORIZATION END ---

            return View(gatePass);
        }

        // GET: DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to delete gate passes.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null || id == 0) return NotFound();

            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();

            // --- CORRECTION FOR DELETE AUTHORIZATION START ---
            if (userRole == "Student" && gatePass.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && gatePass.FacultyId != userId) // Faculty can only delete their own
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            // --- CORRECTION FOR DELETE AUTHORIZATION END ---

            if (gatePass.FinalStatus == "Approved" || gatePass.Status == "Approved")
            {
                TempData["error"] = "Approved gate passes cannot be deleted.";
                return RedirectToAction(nameof(Details), new { id = gatePass.Id });
            }

            return View(gatePass);
        }

        // POST: DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in to delete gate passes.";
                return RedirectToAction("Login", "Account");
            }

            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null)
            {
                return NotFound();
            }

            // --- CORRECTION FOR DELETE POST AUTHORIZATION START ---
            if (userRole == "Student" && gatePass.StudentId != userId)
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            else if (userRole == "Faculty" && gatePass.FacultyId != userId) // Faculty can only delete their own
            {
                TempData["error"] = "You are not authorized to delete this gate pass.";
                return RedirectToAction(nameof(Index));
            }
            // --- CORRECTION FOR DELETE POST AUTHORIZATION END ---

            if (gatePass.FinalStatus == "Approved" || gatePass.Status == "Approved")
            {
                TempData["error"] = "Approved gate passes cannot be deleted.";
                return RedirectToAction(nameof(Details), new { id = gatePass.Id });
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
            await _db.SaveChangesAsync();
            TempData["success"] = "Gate Pass deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}