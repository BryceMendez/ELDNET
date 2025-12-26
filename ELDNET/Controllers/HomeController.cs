using ELDNET.Data;
using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ELDNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private const int MAX_CAR_SLOTS = 50;
        private const int MAX_MOTORCYCLE_SLOTS = 100;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null) return RedirectToAction("Login", "Account");
            if (userRole == "Admin") return RedirectToAction("Index", "Approval");

            int approvedCars = await _db.GatePasses.CountAsync(gp => gp.VehicleType == "Car" && gp.Status == "Approved");
            int approvedMotors = await _db.GatePasses.CountAsync(gp => gp.VehicleType == "Motorcycle" && gp.Status == "Approved");

            ViewBag.CarLeft = Math.Max(0, 50 - approvedCars);
            ViewBag.MotorLeft = Math.Max(0, 100 - approvedMotors);

            var occupiedLockers = await _db.LockerRequests
                .Where(r => r.Status == "Approved")
                .Select(r => r.LockerNumber)
                .ToListAsync();

            ViewBag.OccupiedLockers = occupiedLockers;
            ViewBag.AvailableCount = 100 - occupiedLockers.Count;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}