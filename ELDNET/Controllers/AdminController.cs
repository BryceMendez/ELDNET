using Microsoft.AspNetCore.Mvc;

namespace ELDNET.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetString("UserId");

            if (userRole == null) // not logged in
            {
                TempData["error"] = "You must be logged in to access the admin dashboard.";
                return RedirectToAction("Login", "Account");
            }
            // Strict check for "admin" as UserId, assuming that's the hardcoded admin or an Admin table entry.
            if (!(userRole == "Admin" && userId == "admin"))
            {
                TempData["error"] = "You are not authorized to view this page.";
                return RedirectToAction("Index", "Home"); // Redirect to student dashboard or home for non-admins
            }
            return View();
        }
    }
}