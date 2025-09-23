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

        // GET: LockerRequest
        public IActionResult Index()
        {
            var requests = _context.LockerRequests.ToList();
            return View(requests);
        }

        // GET: LockerRequest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LockerRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LockerRequest request, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            if (ModelState.IsValid)
            {
                // Upload Study Load
                if (studyLoadFile != null && studyLoadFile.Length > 0)
                {
                    string studyLoadPath = Path.Combine("wwwroot/uploads/studyload", studyLoadFile.FileName);
                    using (var stream = new FileStream(studyLoadPath, FileMode.Create))
                    {
                        studyLoadFile.CopyTo(stream);
                    }
                    request.StudyLoadPath = "/uploads/studyload/" + studyLoadFile.FileName;
                }

                // Upload Registration Form
                if (registrationFile != null && registrationFile.Length > 0)
                {
                    string registrationPath = Path.Combine("wwwroot/uploads/registration", registrationFile.FileName);
                    using (var stream = new FileStream(registrationPath, FileMode.Create))
                    {
                        registrationFile.CopyTo(stream);
                    }
                    request.RegistrationPath = "/uploads/registration/" + registrationFile.FileName;
                }

                _context.LockerRequests.Add(request);
                _context.SaveChanges();
                TempData["success"] = "Locker Request created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // GET: LockerRequest/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lockerRequest = _context.LockerRequests.Find(id);
            if (lockerRequest == null)
            {
                return NotFound();
            }
            return View(lockerRequest);
        }

        // POST: LockerRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LockerRequest lockerRequest, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            if (id != lockerRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file uploads for update
                    if (studyLoadFile != null && studyLoadFile.Length > 0)
                    {
                        string studyLoadPath = Path.Combine("wwwroot/uploads/studyload", studyLoadFile.FileName);
                        using (var stream = new FileStream(studyLoadPath, FileMode.Create))
                        {
                            studyLoadFile.CopyTo(stream);
                        }
                        lockerRequest.StudyLoadPath = "/uploads/studyload/" + studyLoadFile.FileName;
                    }

                    if (registrationFile != null && registrationFile.Length > 0)
                    {
                        string registrationPath = Path.Combine("wwwroot/uploads/registration", registrationFile.FileName);
                        using (var stream = new FileStream(registrationPath, FileMode.Create))
                        {
                            registrationFile.CopyTo(stream);
                        }
                        lockerRequest.RegistrationPath = "/uploads/registration/" + registrationFile.FileName;
                    }

                    _context.Update(lockerRequest);
                    _context.SaveChanges();
                    TempData["success"] = "Locker Request updated successfully";
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
            if (id == null)
            {
                return NotFound();
            }

            var lockerRequest = _context.LockerRequests
                .FirstOrDefault(m => m.Id == id);
            if (lockerRequest == null)
            {
                return NotFound();
            }

            return View(lockerRequest);
        }

        // GET: LockerRequest/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lockerRequest = _context.LockerRequests
                .FirstOrDefault(m => m.Id == id);
            if (lockerRequest == null)
            {
                return NotFound();
            }

            return View(lockerRequest);
        }

        // POST: LockerRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var lockerRequest = _context.LockerRequests.Find(id);
            if (lockerRequest != null)
            {
                // Optional: Delete the files from wwwroot if they exist
                if (!string.IsNullOrEmpty(lockerRequest.StudyLoadPath) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.StudyLoadPath.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.StudyLoadPath.TrimStart('/')));
                }
                if (!string.IsNullOrEmpty(lockerRequest.RegistrationPath) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.RegistrationPath.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lockerRequest.RegistrationPath.TrimStart('/')));
                }

                _context.LockerRequests.Remove(lockerRequest);
            }

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