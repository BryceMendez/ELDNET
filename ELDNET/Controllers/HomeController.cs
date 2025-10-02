using ELDNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ELDNET.Controllers
{
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

            if (userRole == null) // not logged in
            {
                return RedirectToAction("Login", "Account");
            }
            else if (userRole == "Admin")
            {
                return RedirectToAction("Index", "Admin"); // Admins go to their dashboard
            }
            else if (userRole == "Student")
            {

                return View(); // Return to the default Home/Index view, which will act as student dashboard
            }
            return View(); // Fallback
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