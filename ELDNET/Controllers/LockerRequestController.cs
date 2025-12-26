using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private List<string> GetFullLockerList()
        {
            var list = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                char rowLetter = (char)('A' + (i / 10));
                int num = (i % 10) + 1;
                list.Add($"{rowLetter}{num:D2}");
            }
            return list;
        }
    public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";
            var userId = HttpContext.Session.GetString("UserId") ?? "";

            if (string.IsNullOrEmpty(userRole))
            {
                TempData["error"] = "You must be logged in.";
                return RedirectToAction("Login", "Account");
            }

            if (userRole == "Admin")
            {
                await CleanupExpiredRequests();
            }

            IQueryable<LockerRequest> requestsQuery = _context.LockerRequests;
            if (userRole == "Student") requestsQuery = requestsQuery.Where(r => r.StudentId == userId);
            else if (userRole == "Faculty") requestsQuery = requestsQuery.Where(r => r.FacultyId == userId);

            var requests = await requestsQuery.OrderByDescending(r => r.Date).ToListAsync();

            var occupiedLockers = await _context.LockerRequests
                .Where(r => r.Status == "Approved")
                .Select(r => r.LockerNumber).ToListAsync();

            ViewBag.OccupiedLockers = occupiedLockers;
            ViewBag.AvailableCount = 100 - occupiedLockers.Count;

            return View(requests);
        }
        public async Task<IActionResult> Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null || userRole == "Admin")
            {
                TempData["error"] = "Admins cannot create locker requests.";
                return RedirectToAction(nameof(Index));
            }

            var occupied = await _context.LockerRequests.Where(r => r.Status == "Approved").Select(r => r.LockerNumber).ToListAsync();
            ViewBag.AvailableLockers = GetFullLockerList().Except(occupied).ToList();

            return View(new LockerRequest
            {
                Name = HttpContext.Session.GetString("FullName") ?? "",
                IdNumber = HttpContext.Session.GetString("UserId") ?? "",
                Date = DateTime.Now,
                Status = "Pending",
                Approver1Name = "Atty. Virgil B. Villanueva (OSD Director)"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LockerRequest request)
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "";
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";

            request.Date = DateTime.Now;
            request.Status = "Pending";
            if (userRole == "Student") request.StudentId = userId;
            else if (userRole == "Faculty") request.FacultyId = userId;

            ModelState.Remove(nameof(request.StudentId));
            ModelState.Remove(nameof(request.FacultyId));

            if (!GetFullLockerList().Contains(request.LockerNumber))
            {
                ModelState.AddModelError("LockerNumber", "Invalid locker number. Please choose from A01 to J10.");
            }
            if (ModelState.IsValid)
            {
                bool hasDuplicate = await _context.LockerRequests.AnyAsync(r =>
                    r.LockerNumber == request.LockerNumber &&
                    r.Status == "Pending" &&
                    (r.StudentId == userId || r.FacultyId == userId));

                if (hasDuplicate)
                {
                    TempData["error"] = "You already have a pending request for this locker.";
                }
                else
                {
                    if (request.StudyLoadFile != null) request.StudyLoadPath = await SaveFile(request.StudyLoadFile, "studyload");
                    if (request.RegistrationFile != null) request.RegistrationPath = await SaveFile(request.RegistrationFile, "registration");

                    _context.LockerRequests.Add(request);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Locker Request submitted successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var occupied = await _context.LockerRequests.Where(r => r.Status == "Approved").Select(r => r.LockerNumber).ToListAsync();
            ViewBag.AvailableLockers = GetFullLockerList().Except(occupied).ToList();
            return View(request);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null) return NotFound();

            var userId = HttpContext.Session.GetString("UserId") ?? "";
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";

            if (userRole != "Admin" && lockerRequest.StudentId != userId && lockerRequest.FacultyId != userId)
                return RedirectToAction(nameof(Index));

            return View(lockerRequest);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LockerRequest request, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            if (id != request.Id) return NotFound();

            var userId = HttpContext.Session.GetString("UserId") ?? "";
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";

            ModelState.Remove("StudyLoadFile");
            ModelState.Remove("RegistrationFile");
            ModelState.Remove(nameof(request.StudentId));
            ModelState.Remove(nameof(request.FacultyId));

            if (!GetFullLockerList().Contains(request.LockerNumber))
            {
                ModelState.AddModelError("LockerNumber", "Invalid locker number. Must be between A01 and J10.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRequest = await _context.LockerRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (existingRequest == null) return NotFound();

                    bool isAlreadyApproved = await _context.LockerRequests.AnyAsync(r =>
                        r.LockerNumber == request.LockerNumber &&
                        r.Status == "Approved" &&
                        r.Id != id);

                    if (isAlreadyApproved)
                    {
                        ModelState.AddModelError("LockerNumber", "This locker number is already approved for another user.");
                        return View(request);
                    }

                    bool isDuplicatePending = await _context.LockerRequests.AnyAsync(r =>
                        r.LockerNumber == request.LockerNumber &&
                        r.Status == "Pending" &&
                        r.Id != id &&
                        (r.StudentId == userId || r.FacultyId == userId));

                    if (isDuplicatePending)
                    {
                        ModelState.AddModelError("LockerNumber", "You already have another pending request for this locker number.");
                        return View(request);
                    }

                    request.StudentId = existingRequest.StudentId;
                    request.FacultyId = existingRequest.FacultyId;
                    request.Date = existingRequest.Date;

                    if (studyLoadFile != null)
                    {
                        DeletePhysicalFile(existingRequest.StudyLoadPath);
                        request.StudyLoadPath = await SaveFile(studyLoadFile, "studyload");
                    }
                    else request.StudyLoadPath = existingRequest.StudyLoadPath;

                    if (registrationFile != null)
                    {
                        DeletePhysicalFile(existingRequest.RegistrationPath);
                        request.RegistrationPath = await SaveFile(registrationFile, "registration");
                    }
                    else request.RegistrationPath = existingRequest.RegistrationPath;

                    if (existingRequest.Status == "Denied")
                    {
                        request.Status = "Pending";
                        request.Approver1Status = "Pending";
                        request.FinalStatus = "Pending";
                        request.DenialReasonLocker = null;
                        request.IsChangedByApplicant = true;
                    }
                    else
                    {
                        request.Status = existingRequest.Status;
                        request.Approver1Status = existingRequest.Approver1Status;
                        request.FinalStatus = existingRequest.FinalStatus;
                    }

                    _context.Update(request);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Request updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LockerRequests.Any(e => e.Id == request.Id)) return NotFound();
                    else throw;
                }
            }
            return View(request);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var lockerRequest = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (lockerRequest == null) return NotFound();

            var userId = HttpContext.Session.GetString("UserId") ?? "";
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";

            if (userRole != "Admin" && lockerRequest.StudentId != userId && lockerRequest.FacultyId != userId)
                return RedirectToAction(nameof(Index));

            if (lockerRequest.Status == "Approved" && userRole != "Admin")
            {
                TempData["error"] = "Approved requests can only be deleted by an Admin.";
                return RedirectToAction(nameof(Index));
            }
            return View(lockerRequest);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lockerRequest = await _context.LockerRequests.FindAsync(id);
            if (lockerRequest == null) return NotFound();

            if (lockerRequest.Status == "Approved" && HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["error"] = "Unauthorized deletion.";
                return RedirectToAction(nameof(Index));
            }

            DeletePhysicalFile(lockerRequest.StudyLoadPath);
            DeletePhysicalFile(lockerRequest.RegistrationPath);
            _context.LockerRequests.Remove(lockerRequest);
            await _context.SaveChangesAsync();
            TempData["success"] = "Request deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        private async Task CleanupExpiredRequests()
        {
            DateTime currentSYStart = (DateTime.Now.Month >= 6) ? new DateTime(DateTime.Now.Year, 6, 1) : new DateTime(DateTime.Now.Year - 1, 6, 1);
            var expired = await _context.LockerRequests.Where(r => r.Date < currentSYStart).ToListAsync();
            if (expired.Any())
            {
                foreach (var req in expired) { DeletePhysicalFile(req.StudyLoadPath); DeletePhysicalFile(req.RegistrationPath); }
                _context.LockerRequests.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }
        }
        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string path = Path.Combine(_hostEnvironment.WebRootPath, "uploads", folder, fileName);
            string? directoryPath = Path.GetDirectoryName(path);
            if (directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
            }
            using (var stream = new FileStream(path, FileMode.Create)) await file.CopyToAsync(stream);
            return $"/uploads/{folder}/{fileName}";
        }
        private void DeletePhysicalFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(_hostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var req = await _context.LockerRequests.FirstOrDefaultAsync(m => m.Id == id);
            if (req == null) return NotFound();
            return View(req);
        }
    }
}