using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session.GetString
using Microsoft.AspNetCore.Authorization; // Required for [Authorize] attribute

namespace ELDNET.Controllers
{

    [Authorize(Roles = "Admin,Student,Faculty")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");


            if (userRole == null) // This condition will likely not be met due to [Authorize]
            {
                return RedirectToAction("Login", "Account");
            }
            else if (userRole == "Admin")
            {
                // Admins go to their dashboard
                return RedirectToAction("Index", "Approval"); // Assuming "Approval" is your Admin controller
            }

            return View(); // Student and Faculty accounts will render the Home/Index view
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