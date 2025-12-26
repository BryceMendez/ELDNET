using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
namespace ELDNET.Controllers
{
    public class GatePassController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
    private const int MAX_CAR_SLOTS = 50;
        private const int MAX_MOTORCYCLE_SLOTS = 100;
        public GatePassController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userRole)) return RedirectToAction("Login", "Account");
            int approvedCars = await _db.GatePasses.CountAsync(gp => gp.VehicleType == "Car" && gp.Status == "Approved");
            int approvedMotors = await _db.GatePasses.CountAsync(gp => gp.VehicleType == "Motorcycle" && gp.Status == "Approved");
            ViewBag.CarLeft = Math.Max(0, MAX_CAR_SLOTS - approvedCars);
            ViewBag.MotorLeft = Math.Max(0, MAX_MOTORCYCLE_SLOTS - approvedMotors);
            IQueryable<GatePass> query = _db.GatePasses;
            if (userRole == "Student") query = query.Where(gp => gp.StudentId == userId);
            else if (userRole == "Faculty") query = query.Where(gp => gp.FacultyId == userId);
            return View(await query.OrderByDescending(gp => gp.Date).ToListAsync());
        }
        #region CREATE
        public IActionResult Create()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole) || userRole == "Admin") return RedirectToAction("Index");
            return View(new GatePass
            {
                Name = HttpContext.Session.GetString("FullName") ?? "Unknown",
                Date = DateTime.Now,
                RegistrationExpiryDate = DateTime.Now.AddYears(1)
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GatePass obj)
        {
            var userId = HttpContext.Session.GetString("UserId") ?? "unknown";
            var userRole = HttpContext.Session.GetString("UserRole") ?? "";
            string inputPlate = obj.PlateNumber?.Replace(" ", "").Replace("-", "").ToUpper() ?? "";
            var existing = await _db.GatePasses.FirstOrDefaultAsync(gp =>
                gp.PlateNumber!.Replace(" ", "").Replace("-", "").ToUpper() == inputPlate &&
                (gp.Status == "Pending" || gp.Status == "Approved"));
            if (existing != null)
            {
                TempData["errorTitle"] = "Duplicate Application";
                TempData["errorMessage"] = $"A request for plate number <b>{obj.PlateNumber}</b> already exists in our system with a <b>{existing.Status}</b> status.";
                return View(obj);
            }
            obj.StudentId = userRole == "Student" ? userId : null;
            obj.FacultyId = userRole == "Faculty" ? userId : null;
            obj.Name = HttpContext.Session.GetString("FullName") ?? "Unknown";
            obj.Date = DateTime.Now;
            obj.Status = "Pending";

            if (obj.StudyLoadFile != null) obj.StudyLoadPath = await SaveFile(obj.StudyLoadFile, userId);
            if (obj.RegistrationFile != null) obj.RegistrationPath = await SaveFile(obj.RegistrationFile, userId);

            ModelState.Remove("Name");
            ModelState.Remove("StudentId");
            ModelState.Remove("FacultyId");
            ModelState.Remove("Status");
            ModelState.Remove("FinalStatus");
            ModelState.Remove("StudyLoadPath");
            ModelState.Remove("RegistrationPath");

            if (ModelState.IsValid)
            {
                _db.GatePasses.Add(obj);
                await _db.SaveChangesAsync();
                TempData["success"] = "Gate Pass submitted successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }
        #endregion
        #region EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();

            if (gatePass.Status != "Pending" && gatePass.Status != "Denied")
            {
                TempData["error"] = "Approved records cannot be edited.";
                return RedirectToAction(nameof(Index));
            }
            return View(gatePass);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GatePass obj)
        {
            var existing = await _db.GatePasses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == obj.Id);
            if (existing == null) return NotFound();

            if (existing.Status != "Pending" && existing.Status != "Denied")
            {
                TempData["error"] = "Approved records cannot be modified.";
                return RedirectToAction(nameof(Index));
            }

            string inputPlate = obj.PlateNumber?.Replace(" ", "").Replace("-", "").ToUpper() ?? "";
            var duplicate = await _db.GatePasses.AnyAsync(gp =>
                gp.Id != obj.Id &&
                gp.PlateNumber!.Replace(" ", "").Replace("-", "").ToUpper() == inputPlate &&
                (gp.Status == "Pending" || gp.Status == "Approved"));

            if (duplicate)
            {
                ModelState.AddModelError("PlateNumber", "Another record already uses this plate number.");
            }

            if (obj.StudyLoadFile != null)
            {
                obj.StudyLoadPath = await SaveFile(obj.StudyLoadFile, obj.StudentId ?? obj.FacultyId ?? "default");
            }

            if (obj.RegistrationFile != null)
            {
                obj.RegistrationPath = await SaveFile(obj.RegistrationFile, obj.StudentId ?? obj.FacultyId ?? "default");
            }
            obj.Status = "Pending";
            obj.DenialReason = null;

            ModelState.Remove("StudyLoadFile");
            ModelState.Remove("RegistrationFile");
            ModelState.Remove("StudentId");
            ModelState.Remove("FacultyId");
            ModelState.Remove("Status");
            ModelState.Remove("Remarks");
            ModelState.Remove("FinalStatus");

            if (ModelState.IsValid)
            {
                try
                {
                    _db.GatePasses.Update(obj);
                    await _db.SaveChangesAsync();
                    TempData["success"] = "Record updated and resubmitted for approval.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes: " + ex.Message);
                }
            }
            return View(obj);
        }
        #endregion
        #region DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();

            if (gatePass.Status != "Pending" && gatePass.Status != "Denied")
            {
                TempData["error"] = "Approved records cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }
            return View(gatePass);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gatePass = await _db.GatePasses.FindAsync(id);

            if (gatePass == null || (gatePass.Status != "Pending" && gatePass.Status != "Denied"))
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(gatePass.StudyLoadPath)) DeletePhysicalFile(gatePass.StudyLoadPath);
            if (!string.IsNullOrEmpty(gatePass.RegistrationPath)) DeletePhysicalFile(gatePass.RegistrationPath);

            _db.GatePasses.Remove(gatePass);
            await _db.SaveChangesAsync();
            TempData["success"] = "Record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var gatePass = await _db.GatePasses.FindAsync(id);
            if (gatePass == null) return NotFound();
            return View(gatePass);
        }
        private async Task<string> SaveFile(IFormFile file, string userId)
        {
            string folder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "gatepass", userId);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create)) { await file.CopyToAsync(stream); }
            return $"/uploads/gatepass/{userId}/{fileName}";
        }
        private void DeletePhysicalFile(string path)
        {
            var fullPath = Path.Combine(_hostEnvironment.WebRootPath, path.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }
    }
}