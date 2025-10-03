using Microsoft.AspNetCore.Mvc;

namespace ELDNET.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");
            if (userRole == null)
            {
                TempData["error"] = "You must be logged in to access the admin dashboard.";
                return RedirectToAction("Login", "Account");
            }
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}