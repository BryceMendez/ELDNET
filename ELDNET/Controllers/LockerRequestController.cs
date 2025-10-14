using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http; // Added for HttpContext.Session
using System; // Added for DateTime
using System.Threading.Tasks; // Added for async/await

namespace ELDNET.Controllers
{
    public class LockerRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LockerRequestController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index() // Made async
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId"); // This holds StudentId or FacultyId

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view locker requests.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<LockerRequest> requestsQuery = _context.LockerRequests;

            if (userRole == "Student")
            {
                requestsQuery = requestsQuery.Where(r => r.StudentId == userId);
            }
            // ASSUMPTION: Faculty can also create locker requests and should only see their own.
            // If faculty's role is purely for APPROVAL, this logic needs adjustment
            else if (userRole == "Faculty")
            {
                // Assuming LockerRequest model has a FacultyId property
                requestsQuery = requestsQuery.Where(r => r.FacultyId == userId);
            }
            // Admins see all requests, no additional filtering needed for them.

            var requests = await requestsQuery.OrderByDescending(r => r.Date).ToListAsync(); // Use await

            return View(requests);
        }

        // GET: LockerRequest/Create
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create locker requests."; // Adjusted for faculty
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var model = new LockerRequest();
            model.Name = fullName;
            model.IdNumber = userId; // Assuming IdNumber can store both student and faculty IDs
            model.Date = DateTime.Now;

            // Set the correct ID based on the user role
            if (userRole == "Student")
            {
                model.StudentId = userId;
            }
            else if (userRole == "Faculty")
            {
                model.FacultyId = userId;
                // You might want to clear StudentId if it's not relevant for faculty requests
                model.StudentId = null;
            }

            model.Status = "Pending";
            model.FinalStatus = "Pending";
            model.Approver1Status = "Pending";
            model.Approver1Name = "Atty. Virgil B. Villanueva (OSD Director)"; // Ensure this is also set

            return View(model);
        }

        // POST: LockerRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LockerRequest request)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create locker requests."; // Adjusted for faculty
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User information (ID or Full Name) missing from session. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            // Manually set system-managed fields
            request.Date = DateTime.Now;
            request.Status = "Pending";
            request.FinalStatus = "Pending";
            request.Approver1Status = "Pending";
            request.Approver1Name = "Atty. Virgil B. Villanueva (OSD Director)";

            // Set the correct ID based on the user role
            if (userRole == "Student")
            {
                request.StudentId = userId;
                request.FacultyId = null; // Ensure faculty ID is null for student requests
            }
            else if (userRole == "Faculty")
            {
                request.FacultyId = userId;
                request.StudentId = null; // Ensure student ID is null for faculty requests
            }

            // Remove ModelState entries for properties that are definitively set by the system
            ModelState.Remove(nameof(request.StudentId));
            ModelState.Remove(nameof(request.FacultyId)); // Also remove FacultyId from validation
            ModelState.Remove(nameof(request.Date));
            ModelState.Remove(nameof(request.Status));
            ModelState.Remove(nameof(request.FinalStatus));
            ModelState.Remove(nameof(request.Approver1Status));
            ModelState.Remove(nameof(request.Approver1Name));
            ModelState.Remove(nameof(request.Name)); // Assuming Name is purely from session
            ModelState.Remove(nameof(request.IdNumber)); // Assuming IdNumber is purely from session

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
                string registrationFolder = Path.Combine(uploadsFolder, "registration");

                if (!Directory.Exists(studyLoadFolder)) Directory.CreateDirectory(studyLoadFolder);
                if (!Directory.Exists(registrationFolder)) Directory.CreateDirectory(registrationFolder);

                // Upload Study Load
                if (request.StudyLoadFile != null && request.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{userId}_{Guid.NewGuid()}_{request.StudyLoadFile.FileName}";
                    string studyLoadPath = Path.Combine(studyLoadFolder, uniqueFileName);
                    using (var stream = new FileStream(studyLoadPath, FileMode.Create))
                    {
                        await request.StudyLoadFile.CopyToAsync(stream);
                    }
                    request.StudyLoadPath = $"/uploads/studyload/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(request.StudyLoadFile), "Please upload the Study Load file.");
                    // Re-populate session-derived fields for the view before returning
                    request.Name = fullName;
                    request.IdNumber = userId;
                    return View(request);
                }

                // Upload Registration Form
                if (request.RegistrationFile != null && request.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{userId}_{Guid.NewGuid()}_{request.RegistrationFile.FileName}";
                    string registrationPath = Path.Combine(registrationFolder, uniqueFileName);
                    using (var stream = new FileStream(registrationPath, FileMode.Create))
                    {
                        await request.RegistrationFile.CopyToAsync(stream);
                    }
                    request.RegistrationPath = $"/uploads/registration/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(request.RegistrationFile), "Please upload the Registration Form file.");
                    // Re-populate session-derived fields for the view before returning
                    request.Name = fullName;
                    request.IdNumber = userId;
                    return View(request);
                }

                _context.LockerRequests.Add(request);
                await _context.SaveChangesAsync();
                TempData["success"] = "Locker Request created successfully";
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, re-populate session-derived fields for the view
            request.Name = fullName;
            request.IdNumber = userId;

            return View(request);
        }

        // GET: LockerRequest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null) return NotFound();

            bool authorized = false;
            if (userRole == "Student" && lockerRequest.StudentId == userId)
            {
                authorized = true;
            }
            else if (userRole == "Faculty" && lockerRequest.FacultyId == userId) // Added faculty authorization
            {
                authorized = true;
            }
            else if (userRole == "Admin") // Admin can edit anything
            {
                authorized = true;
            }

            if (!authorized)
            {
                TempData["error"] = "You are not authorized to edit this locker request.";
                return RedirectToAction(nameof(Index));
            }

            if (lockerRequest.FinalStatus == "Approved" || lockerRequest.Status == "Approved")
            {
                TempData["info"] = "Cannot edit an approved locker request. Viewing details instead.";
                return RedirectToAction(nameof(Details), new { id = lockerRequest.Id });
            }

            return View(lockerRequest);
        }

        // POST: LockerRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LockerRequest lockerRequest)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName"); // Added to repopulate on validation fail

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to edit locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id != lockerRequest.Id) return NotFound();

            var originalRequest = await _context.LockerRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (originalRequest == null) return NotFound();

            bool authorized = false;
            if (userRole == "Student" && originalRequest.StudentId == userId)
            {
                authorized = true;
            }
            else if (userRole == "Faculty" && originalRequest.FacultyId == userId) // Added faculty authorization
            {
                authorized = true;
            }
            else if (userRole == "Admin")
            {
                authorized = true;
            }

            if (!authorized)
            {
                TempData["error"] = "You are not authorized to edit this locker request.";
                return RedirectToAction(nameof(Index));
            }

            if (originalRequest.FinalStatus == "Approved" || originalRequest.Status == "Approved")
            {
                TempData["info"] = "Cannot edit an approved locker request. Viewing details instead.";
                return RedirectToAction(nameof(Details), new { id = lockerRequest.Id });
            }

            // Assign system-managed properties from the original request
            lockerRequest.StudentId = originalRequest.StudentId;
            lockerRequest.FacultyId = originalRequest.FacultyId; // Preserve FacultyId
            lockerRequest.Date = originalRequest.Date;
            lockerRequest.Status = "Changed";
            lockerRequest.FinalStatus = "Changed";
            lockerRequest.Approver1Status = "Pending";
            lockerRequest.Approver1Name = originalRequest.Approver1Name;

            // Clear ModelState entries for properties manually set
            ModelState.Remove(nameof(lockerRequest.StudentId));
            ModelState.Remove(nameof(lockerRequest.FacultyId)); // Remove from validation
            ModelState.Remove(nameof(lockerRequest.Date));
            ModelState.Remove(nameof(lockerRequest.Status));
            ModelState.Remove(nameof(lockerRequest.FinalStatus));
            ModelState.Remove(nameof(lockerRequest.Approver1Status));
            ModelState.Remove(nameof(lockerRequest.Approver1Name));
            ModelState.Remove(nameof(lockerRequest.Name)); // Assuming Name is purely from session/original
            ModelState.Remove(nameof(lockerRequest.IdNumber)); // Assuming IdNumber is purely from session/original

            string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
            string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
            string registrationFolder = Path.Combine(uploadsFolder, "registration");

            if (!Directory.Exists(studyLoadFolder)) Directory.CreateDirectory(studyLoadFolder);
            if (!Directory.Exists(registrationFolder)) Directory.CreateDirectory(registrationFolder);

            // Handle Study Load file
            if (lockerRequest.StudyLoadFile != null && lockerRequest.StudyLoadFile.Length > 0)
            {
                string uniqueFileName = $"{(originalRequest.StudentId ?? originalRequest.FacultyId)}_{Guid.NewGuid()}_{lockerRequest.StudyLoadFile.FileName}";
                string filePath = Path.Combine(studyLoadFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lockerRequest.StudyLoadFile.CopyToAsync(stream);
                }
                lockerRequest.StudyLoadPath = $"/uploads/studyload/{uniqueFileName}";
            }
            else
            {
                lockerRequest.StudyLoadPath = originalRequest.StudyLoadPath;
            }

            // Handle Registration Form file
            if (lockerRequest.RegistrationFile != null && lockerRequest.RegistrationFile.Length > 0)
            {
                string uniqueFileName = $"{(originalRequest.StudentId ?? originalRequest.FacultyId)}_{Guid.NewGuid()}_{lockerRequest.RegistrationFile.FileName}";
                string filePath = Path.Combine(registrationFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await lockerRequest.RegistrationFile.CopyToAsync(stream);
                }
                lockerRequest.RegistrationPath = $"/uploads/registration/{uniqueFileName}";
            }
            else
            {
                lockerRequest.RegistrationPath = originalRequest.RegistrationPath;
            }

            if (string.IsNullOrEmpty(lockerRequest.StudyLoadPath))
            {
                ModelState.AddModelError(nameof(lockerRequest.StudyLoadFile), "Study Load file is required.");
            }
            if (string.IsNullOrEmpty(lockerRequest.RegistrationPath))
            {
                ModelState.AddModelError(nameof(lockerRequest.RegistrationFile), "Registration Form file is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lockerRequest);
                    await _context.SaveChangesAsync();
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

            // If ModelState is invalid, re-populate session-derived fields for the view
            lockerRequest.Name = fullName;
            lockerRequest.IdNumber = userId; // Repopulate current user ID for the form

            return View(lockerRequest);
        }

        // GET: LockerRequest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view locker request details.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            bool authorized = false;
            if (userRole == "Student" && lockerRequest.StudentId == userId)
            {
                authorized = true;
            }
            else if (userRole == "Faculty" && lockerRequest.FacultyId == userId) // Added faculty authorization
            {
                authorized = true;
            }
            else if (userRole == "Admin")
            {
                authorized = true;
            }

            if (!authorized)
            {
                TempData["error"] = "You are not authorized to view this locker request.";
                return RedirectToAction(nameof(Index));
            }

            return View(lockerRequest);
        }

        // GET: LockerRequest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete locker requests.";
                return RedirectToAction("Login", "Account");
            }

            if (id == null) return NotFound();

            var lockerRequest = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            bool authorized = false;
            if (userRole == "Student" && lockerRequest.StudentId == userId)
            {
                authorized = true;
            }
            else if (userRole == "Faculty" && lockerRequest.FacultyId == userId) // Added faculty authorization
            {
                authorized = true;
            }
            else if (userRole == "Admin")
            {
                authorized = true;
            }

            if (!authorized)
            {
                TempData["error"] = "You are not authorized to delete this locker request.";
                return RedirectToAction(nameof(Index));
            }

            return View(lockerRequest);
        }

        // POST: LockerRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to delete locker requests.";
                return RedirectToAction("Login", "Account");
            }

            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null)
            {
                return NotFound();
            }

            bool authorized = false;
            if (userRole == "Student" && lockerRequest.StudentId == userId)
            {
                authorized = true;
            }
            else if (userRole == "Faculty" && lockerRequest.FacultyId == userId) // Added faculty authorization
            {
                authorized = true;
            }
            else if (userRole == "Admin")
            {
                authorized = true;
            }

            if (!authorized)
            {
                TempData["error"] = "You are not authorized to delete this locker request.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(lockerRequest.StudyLoadPath))
            {
                string fullPath = Path.Combine(_hostEnvironment.WebRootPath, lockerRequest.StudyLoadPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            if (!string.IsNullOrEmpty(lockerRequest.RegistrationPath))
            {
                string fullPath = Path.Combine(_hostEnvironment.WebRootPath, lockerRequest.RegistrationPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.LockerRequests.Remove(lockerRequest);
            await _context.SaveChangesAsync();
            TempData["success"] = "Locker Request deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool LockerRequestExists(int id)
        {
            return _context.LockerRequests.Any(e => e.Id == id);
        }
    }
}