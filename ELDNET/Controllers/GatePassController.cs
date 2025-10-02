using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;

namespace ELDNET.Controllers
{
    public class GatePassController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GatePassController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null) // not logged in
            {
                return RedirectToAction("Login", "Account");
            }
            IEnumerable<GatePass> gatePassList = _db.GatePasses;
            return RedirectToAction("Create");
        }

        // GET: CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GatePass obj)
        {
            if (ModelState.IsValid)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Save Study Load file
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string filePath = Path.Combine(uploadFolder, obj.StudyLoadFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.StudyLoadFile.CopyTo(stream);
                    }
                    obj.StudyLoadPath = "/uploads/" + obj.StudyLoadFile.FileName;
                }

                // Save Registration Form file
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string filePath = Path.Combine(uploadFolder, obj.RegistrationFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.RegistrationFile.CopyTo(stream);
                    }
                    obj.RegistrationPath = "/uploads/" + obj.RegistrationFile.FileName;
                }

                _db.GatePasses.Add(obj);
                _db.SaveChanges();

                TempData["success"] = "Gate Pass created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            return View(gatePass);
        }

        // POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GatePass obj)
        {
            if (ModelState.IsValid)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // Update Study Load file if uploaded
                if (obj.StudyLoadFile != null && obj.StudyLoadFile.Length > 0)
                {
                    string filePath = Path.Combine(uploadFolder, obj.StudyLoadFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.StudyLoadFile.CopyTo(stream);
                    }
                    obj.StudyLoadPath = "/uploads/" + obj.StudyLoadFile.FileName;
                }

                // Update Registration Form file if uploaded
                if (obj.RegistrationFile != null && obj.RegistrationFile.Length > 0)
                {
                    string filePath = Path.Combine(uploadFolder, obj.RegistrationFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        obj.RegistrationFile.CopyTo(stream);
                    }
                    obj.RegistrationPath = "/uploads/" + obj.RegistrationFile.FileName;
                }

                _db.GatePasses.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Gate Pass updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: DETAILS
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            return View(gatePass);
        }

        // GET: DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            return View(gatePass);
        }

        // POST: DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var gatePass = _db.GatePasses.Find(id);
            if (gatePass == null) return NotFound();

            _db.GatePasses.Remove(gatePass);
            _db.SaveChanges();
            TempData["success"] = "Gate Pass deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
