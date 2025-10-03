using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Security.Cryptography;
using System.Text;

namespace ELDNET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            ViewData["FullScreen"] = true;
            ViewData["HideFooter"] = true;
            ViewData["HideNav"] = true;

            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            ViewData["HideNav"] = true;
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", "admin");
                return RedirectToAction("Index", "Approval");
            }
            var admin = _db.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", admin.Username);
                return RedirectToAction("Index", "Approval");
            }
            var student = _db.StudentAccounts
                .FirstOrDefault(s => s.StudentId == username || s.Email == username);
            if (student != null)
            {
                string hashedPassword = HashPassword(password);
                if (student.PasswordHash == hashedPassword)
                {
                    HttpContext.Session.SetString("UserRole", "Student");
                    HttpContext.Session.SetString("UserId", student.StudentId);
                    HttpContext.Session.SetString("FullName", student.FullName);
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = "Invalid username or password.";
            return RedirectToAction("Login");
        }
        public IActionResult SignUp()
        {
            ViewData["HideNav"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string fullName, string email, string password, string confirmPassword)
        {
            ViewData["HideNav"] = true;

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }
            if (_db.StudentAccounts.Any(sa => sa.Email == email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }
            string studentId;
            do
            {
                studentId = "ucb-" + new Random().Next(100000, 999999).ToString();
            }
            while (_db.StudentAccounts.Any(sa => sa.StudentId == studentId));

            string hashedPassword = HashPassword(password);

            var studentAccount = new StudentAccount
            {
                StudentId = studentId,
                FullName = fullName,
                Email = email,
                PasswordHash = hashedPassword
            };

            _db.StudentAccounts.Add(studentAccount);
            _db.SaveChanges();

            TempData["Message"] = $"Account created! Your Student ID is {studentId}. Use it to log in.";
            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }                    
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}