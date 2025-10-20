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

        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to view locker requests.";
                return RedirectToAction("Login", "Account");
            }

            IQueryable<LockerRequest> requestsQuery = _context.LockerRequests;

            if (userRole == "Student")
                requestsQuery = requestsQuery.Where(r => r.StudentId == userId);
            else if (userRole == "Faculty")
                requestsQuery = requestsQuery.Where(r => r.FacultyId == userId);

            var requests = await requestsQuery.OrderByDescending(r => r.Date).ToListAsync();
            return View(requests);
        }

        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var fullName = HttpContext.Session.GetString("FullName");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create locker requests.";
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                TempData["error"] = "User info missing. Please log in again.";
                return RedirectToAction("Login", "Account");
            }

            var model = new LockerRequest
            {
                Name = fullName,
                IdNumber = userId,
                Date = DateTime.Now,
                Status = "Pending",
                FinalStatus = "Pending",
                Approver1Status = "Pending",
                Approver1Name = "Atty. Virgil B. Villanueva (OSD Director)"
            };

            if (userRole == "Student")
                model.StudentId = userId;
            else if (userRole == "Faculty")
                model.FacultyId = userId;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LockerRequest request)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName");

            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Only students or faculty can create locker requests.";
                return RedirectToAction("Index", "Home");
            }

            request.Date = DateTime.Now;
            request.Status = "Pending";
            request.FinalStatus = "Pending";
            request.Approver1Status = "Pending";
            request.Approver1Name = "Atty. Virgil B. Villanueva (OSD Director)";

            if (userRole == "Student")
                request.StudentId = userId;
            else if (userRole == "Faculty")
                request.FacultyId = userId;

            ModelState.Remove(nameof(request.StudentId));
            ModelState.Remove(nameof(request.FacultyId));
            ModelState.Remove(nameof(request.Date));
            ModelState.Remove(nameof(request.Status));
            ModelState.Remove(nameof(request.FinalStatus));
            ModelState.Remove(nameof(request.Approver1Status));
            ModelState.Remove(nameof(request.Approver1Name));
            ModelState.Remove(nameof(request.Name));
            ModelState.Remove(nameof(request.IdNumber));

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
                string registrationFolder = Path.Combine(uploadsFolder, "registration");
                Directory.CreateDirectory(studyLoadFolder);
                Directory.CreateDirectory(registrationFolder);

                if (request.StudyLoadFile != null && request.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{userId}_{Guid.NewGuid()}_{request.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(studyLoadFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await request.StudyLoadFile.CopyToAsync(stream);
                    request.StudyLoadPath = $"/uploads/studyload/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(request.StudyLoadFile), "Please upload the Study Load file.");
                    request.Name = fullName;
                    request.IdNumber = userId;
                    return View(request);
                }

                if (request.RegistrationFile != null && request.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{userId}_{Guid.NewGuid()}_{request.RegistrationFile.FileName}";
                    string filePath = Path.Combine(registrationFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await request.RegistrationFile.CopyToAsync(stream);
                    request.RegistrationPath = $"/uploads/registration/{uniqueFileName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(request.RegistrationFile), "Please upload the Registration Form file.");
                    request.Name = fullName;
                    request.IdNumber = userId;
                    return View(request);
                }

                _context.LockerRequests.Add(request);
                await _context.SaveChangesAsync();
                TempData["success"] = "Locker Request created successfully.";
                return RedirectToAction(nameof(Index));
            }

            request.Name = fullName;
            request.IdNumber = userId;
            return View(request);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null) return NotFound();

            bool authorized =
                (userRole == "Student" && lockerRequest.StudentId == userId) ||
                (userRole == "Faculty" && lockerRequest.FacultyId == userId) ||
                (userRole == "Admin");

            if (!authorized)
            {
                TempData["error"] = "Not authorized to edit this request.";
                return RedirectToAction(nameof(Index));
            }

            if (lockerRequest.Status == "Approved" || lockerRequest.FinalStatus == "Approved")
            {
                TempData["info"] = "Cannot edit approved requests.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(lockerRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LockerRequest lockerRequest)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            var fullName = HttpContext.Session.GetString("FullName");

            if (id != lockerRequest.Id) return NotFound();

            var original = await _context.LockerRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (original == null) return NotFound();

            bool authorized =
                (userRole == "Student" && original.StudentId == userId) ||
                (userRole == "Faculty" && original.FacultyId == userId) ||
                (userRole == "Admin");

            if (!authorized)
            {
                TempData["error"] = "Not authorized to edit this request.";
                return RedirectToAction(nameof(Index));
            }

            if (original.Status == "Approved" || original.FinalStatus == "Approved")
            {
                TempData["info"] = "Cannot edit approved requests.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Preserve system and original fields
            lockerRequest.StudentId = original.StudentId;
            lockerRequest.FacultyId = original.FacultyId;
            lockerRequest.Date = original.Date;
            lockerRequest.Status = "Changed";
            lockerRequest.FinalStatus = "Changed";
            lockerRequest.Approver1Status = "Pending";
            lockerRequest.Approver1Name = original.Approver1Name;

            // ✅ Important: clear validation for files when none uploaded
            if (lockerRequest.StudyLoadFile == null)
                ModelState.Remove(nameof(lockerRequest.StudyLoadFile));
            if (lockerRequest.RegistrationFile == null)
                ModelState.Remove(nameof(lockerRequest.RegistrationFile));

            if (ModelState.IsValid)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
                string studyLoadFolder = Path.Combine(uploadsFolder, "studyload");
                string registrationFolder = Path.Combine(uploadsFolder, "registration");
                Directory.CreateDirectory(studyLoadFolder);
                Directory.CreateDirectory(registrationFolder);

                // ✅ Keep old file if no new one uploaded
                if (lockerRequest.StudyLoadFile != null && lockerRequest.StudyLoadFile.Length > 0)
                {
                    string uniqueFileName = $"{(original.StudentId ?? original.FacultyId)}_{Guid.NewGuid()}_{lockerRequest.StudyLoadFile.FileName}";
                    string filePath = Path.Combine(studyLoadFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await lockerRequest.StudyLoadFile.CopyToAsync(stream);
                    lockerRequest.StudyLoadPath = $"/uploads/studyload/{uniqueFileName}";
                }
                else
                {
                    lockerRequest.StudyLoadPath = original.StudyLoadPath;
                }

                if (lockerRequest.RegistrationFile != null && lockerRequest.RegistrationFile.Length > 0)
                {
                    string uniqueFileName = $"{(original.StudentId ?? original.FacultyId)}_{Guid.NewGuid()}_{lockerRequest.RegistrationFile.FileName}";
                    string filePath = Path.Combine(registrationFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await lockerRequest.RegistrationFile.CopyToAsync(stream);
                    lockerRequest.RegistrationPath = $"/uploads/registration/{uniqueFileName}";
                }
                else
                {
                    lockerRequest.RegistrationPath = original.RegistrationPath;
                }

                _context.Update(lockerRequest);
                await _context.SaveChangesAsync();

                TempData["success"] = "Locker Request updated successfully. Status set to 'Changed' for re-approval.";
                return RedirectToAction(nameof(Index));
            }

            // Refill identity info in case of validation errors
            lockerRequest.Name = fullName;
            lockerRequest.IdNumber = userId;
            return View(lockerRequest);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var lockerRequest = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            bool authorized =
                (userRole == "Student" && lockerRequest.StudentId == userId) ||
                (userRole == "Faculty" && lockerRequest.FacultyId == userId) ||
                (userRole == "Admin");

            if (!authorized)
            {
                TempData["error"] = "Not authorized to view this request.";
                return RedirectToAction(nameof(Index));
            }

            return View(lockerRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var lockerRequest = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (lockerRequest == null) return NotFound();
            return View(lockerRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null) return NotFound();

            if (!string.IsNullOrEmpty(lockerRequest.StudyLoadPath))
            {
                string fullPath = Path.Combine(_hostEnvironment.WebRootPath, lockerRequest.StudyLoadPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }

            if (!string.IsNullOrEmpty(lockerRequest.RegistrationPath))
            {
                string fullPath = Path.Combine(_hostEnvironment.WebRootPath, lockerRequest.RegistrationPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            }

            _context.LockerRequests.Remove(lockerRequest);
            await _context.SaveChangesAsync();
            TempData["success"] = "Locker Request deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool LockerRequestExists(int id)
        {
            return _context.LockerRequests.Any(e => e.Id == id);
        }
    }
}
