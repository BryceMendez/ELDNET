using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;

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
            var requests = _context.LockerRequests.ToList();
            return View(requests);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LockerRequest request, IFormFile? studyLoadFile, IFormFile? registrationFile)
        {
            if (ModelState.IsValid)
            {
                // ✅ Upload Study Load
                if (studyLoadFile != null && studyLoadFile.Length > 0)
                {
                    string studyLoadPath = Path.Combine("wwwroot/uploads/studyload", studyLoadFile.FileName);
                    using (var stream = new FileStream(studyLoadPath, FileMode.Create))
                    {
                        studyLoadFile.CopyTo(stream);
                    }
                    request.StudyLoadPath = "/uploads/studyload/" + studyLoadFile.FileName;
                }

                // ✅ Upload Registration Form
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
    }
}
