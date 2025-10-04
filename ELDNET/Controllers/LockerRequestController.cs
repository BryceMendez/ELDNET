using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ELDNET.Controllers
{
    public class LockerRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LockerRequestController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to view locker requests.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<LockerRequest> requestsQuery = _context.LockerRequests;

            if (userRole == "Student")
            {
 
                requestsQuery = requestsQuery.Where(r => r.StudentId == studentId);
            }
 
            var requests = requestsQuery.OrderByDescending(r => r.Date).ToList();
  

            return View(requests); // Return to a proper Index view
        }

        // GET: LockerRequest/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin") // Only students can create requests
            {
                TempData["error"] = "Only students can create locker requests.";
                return RedirectToAction("Index", "Home"); // Or another appropriate page
            }

            // Pre-fill Name and IDNumber if available from session
            var model = new LockerRequest();
            if (fullName != null)
            {
                model.Name = fullName;
            }
            if (studentId != null && studentId.StartsWith("ucb-")) // Assuming StudentId is also the ID Number
            {
                model.IdNumber = studentId;
            }
            model.Date = DateTime.Now; // Pre-fill submission date

            return View(model);
        }

        // POST: LockerRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LockerRequest request, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students can create locker requests.";
                return RedirectToAction("Index", "Home");
            }

            // Assign the StudentId from the session
            request.StudentId = studentId;
            request.Date = DateTime.Now; // Set submission date on creation

            // Remove ModelState entries for properties managed by the system
            ModelState.Remove(nameof(request.Name));
            ModelState.Remove(nameof(request.IdNumber));
            ModelState.Remove(nameof(request.StudentId));
            ModelState.Remove(nameof(request.Date));
            ModelState.Remove(nameof(request.Status));
            ModelState.Remove(nameof(request.FinalStatus));
            ModelState.Remove(nameof(request.Approver1Status));



            if (ModelState.IsValid)
            {
                // Upload Study Load
                if (studyLoadFile != null && studyLoadFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
                    if (!Directory.Exists(studyLoadFolder)) Directory.CreateDirectory(studyLoadFolder);

                    // Use a unique file name to avoid clashes, e.g., combine with StudentId or a GUID
                    string uniqueFileName = $"{studentId}_{Guid.NewGuid().ToString()}_{studyLoadFile.FileName}";
                    string studyLoadPath = Path.Combine(studyLoadFolder, uniqueFileName);
                    using (var stream = new FileStream(studyLoadPath, FileMode.Create))
                    {
                        studyLoadFile.CopyTo(stream);
                    }
                    request.StudyLoadPath = "/uploads/studyload/" + uniqueFileName;
                }

                // Upload Registration Form
                if (registrationFile != null && registrationFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string registrationFolder = Path.Combine(uploadsFolder, "registration");
                    if (!Directory.Exists(registrationFolder)) Directory.CreateDirectory(registrationFolder);

                    string uniqueFileName = $"{studentId}_{Guid.NewGuid().ToString()}_{registrationFile.FileName}";
                    string registrationPath = Path.Combine(registrationFolder, uniqueFileName);
                    using (var stream = new FileStream(registrationPath, FileMode.Create))
                    {
                        registrationFile.CopyTo(stream);
                    }
                    request.RegistrationPath = "/uploads/registration/" + uniqueFileName;
                }

                _context.LockerRequests.Add(request);
                _context.SaveChanges();
                TempData["success"] = "Locker Request created successfully";
                return RedirectToAction(nameof(Index));
            }

            // Re-assign pre-filled values if ModelState is invalid to keep them in the form
            var fullName = HttpContext.Session.GetString("FullName");
            if (fullName != null) request.Name = fullName;
            if (studentId != null && studentId.StartsWith("ucb-")) request.IdNumber = studentId;
            request.Date = DateTime.Now; // Re-set submission date

            return View(request);
        }

        // GET: LockerRequest/Edit/5
        public IActionResult Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = _context.LockerRequests.Find(id);
            if (lockerRequest == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && lockerRequest.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this locker request.";
                return RedirectToAction(nameof(Index));
            }

            // --- NEW: Prevent editing if approved ---
            if (lockerRequest.FinalStatus == "Approved" || lockerRequest.Status == "Approved")
            {
                return RedirectToAction(nameof(Details), new { id = lockerRequest.Id });
            }


            return View(lockerRequest);
        }

        // POST: LockerRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LockerRequest lockerRequest, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id != lockerRequest.Id) return NotFound();

            // Retrieve the original request to preserve StudentId and prevent tampering
            var originalRequest = _context.LockerRequests.AsNoTracking().FirstOrDefault(r => r.Id == id);
            if (originalRequest == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && originalRequest.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to edit this locker request.";
                return RedirectToAction(nameof(Index));
            }


            if (originalRequest.FinalStatus == "Approved" || originalRequest.Status == "Approved")
            {
                return RedirectToAction(nameof(Details), new { id = lockerRequest.Id });
            }
            lockerRequest.StudentId = originalRequest.StudentId;
            lockerRequest.Date = originalRequest.Date; // Preserve original submission date
            lockerRequest.Status = "Changed";
            lockerRequest.FinalStatus = "Changed";
            lockerRequest.Approver1Status = "Pending";

            // Clear specific ModelState entries that are now being manually set
            ModelState.Remove(nameof(lockerRequest.StudentId));
            ModelState.Remove(nameof(lockerRequest.Date)); // Ensure this isn't validated as user didn't set it
            ModelState.Remove(nameof(lockerRequest.Status));
            ModelState.Remove(nameof(lockerRequest.FinalStatus));
            ModelState.Remove(nameof(lockerRequest.Approver1Status));



            if (ModelState.IsValid)
            {
                try
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
                    string registrationFolder = Path.Combine(uploadsFolder, "registration");

                    // Handle file uploads for update
                    if (studyLoadFile != null && studyLoadFile.Length > 0)
                    {
                        if (!Directory.Exists(studyLoadFolder)) Directory.CreateDirectory(studyLoadFolder);
                        string uniqueFileName = $"{originalRequest.StudentId}_{Guid.NewGuid().ToString()}_{studyLoadFile.FileName}";
                        string filePath = Path.Combine(studyLoadFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            studyLoadFile.CopyTo(stream);
                        }
                        lockerRequest.StudyLoadPath = "/uploads/studyload/" + uniqueFileName;
                    }
                    else
                    {
                        // Preserve existing file path if no new file is uploaded
                        lockerRequest.StudyLoadPath = originalRequest.StudyLoadPath;
                    }

                    if (registrationFile != null && registrationFile.Length > 0)
                    {
                        if (!Directory.Exists(registrationFolder)) Directory.CreateDirectory(registrationFolder);
                        string uniqueFileName = $"{originalRequest.StudentId}_{Guid.NewGuid().ToString()}_{registrationFile.FileName}";
                        string filePath = Path.Combine(registrationFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            registrationFile.CopyTo(stream);
                        }
                        lockerRequest.RegistrationPath = "/uploads/registration/" + uniqueFileName;
                    }
                    else
                    {
                        // Preserve existing file path if no new file is uploaded
                        lockerRequest.RegistrationPath = originalRequest.RegistrationPath;
                    }

                    _context.Update(lockerRequest);
                    _context.SaveChanges();
                    TempData["success"] = "Locker Request updated successfully. Status changed to 'Changed' - requires re-approval.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LockerRequestExists(lockerRequest.Id))
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
            return View(lockerRequest);
        }

        // GET: LockerRequest/Details/5
        public IActionResult Details(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view locker request details.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = _context.LockerRequests.FirstOrDefault(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && lockerRequest.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to view this locker request.";
                return RedirectToAction(nameof(Index));
            }

            return View(lockerRequest);
        }

        // GET: LockerRequest/Delete/5
        public IActionResult Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = _context.LockerRequests.FirstOrDefault(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            // Authorization check
            if (userRole == "Student" && lockerRequest.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this locker request.";
                return RedirectToAction(nameof(Index));
            }

            return View(lockerRequest);
        }

        // POST: LockerRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var studentId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete locker requests.";
                return RedirectToAction("Login", "Account");
            }

            var lockerRequest = _context.LockerRequests.Find(id);
            if (lockerRequest == null)
            {
                return NotFound();
            }

            // Authorization check
            if (userRole == "Student" && lockerRequest.StudentId != studentId)
            {
                TempData["error"] = "You are not authorized to delete this locker request.";
                return RedirectToAction(nameof(Index));
            }

            // Optional: Delete the files from wwwroot if they exist
            if (!string.IsNullOrEmpty(lockerRequest.StudyLoadPath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.StudyLoadPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            if (!string.IsNullOrEmpty(lockerRequest.RegistrationPath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.RegistrationPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.LockerRequests.Remove(lockerRequest);
            _context.SaveChanges();
            TempData["success"] = "Locker Request deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool LockerRequestExists(int id)
        {
            return _context.LockerRequests.Any(e => e.Id == id);
        }
    }
}