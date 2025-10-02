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

        // GET: Login
        public IActionResult Login()
        {
            ViewData["FullScreen"] = true;
            ViewData["HideFooter"] = true;
            ViewData["HideNav"] = true; // hide navbar
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            ViewData["HideNav"] = true; // keep nav hidden if login fails

            // 1️⃣ Check hardcoded Admin (optional fallback)
            if (username == "admin" && password == "Admin@001")
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", "admin");
                return RedirectToAction("Index", "Admin"); // send to admin dashboard
            }

            // 2️⃣ Check Admin table (if you have one)
            var admin = _db.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", admin.Username);
                return RedirectToAction("Index", "Admin");
            }

            // 3️⃣ Check StudentAccounts table
            var student = _db.StudentAccounts
                .FirstOrDefault(s => s.StudentId == username || s.Email == username);

            if (student != null)
            {
                string hashedPassword = HashPassword(password);
                if (student.PasswordHash == hashedPassword)
                {
                    HttpContext.Session.SetString("UserRole", "Student");
                    HttpContext.Session.SetString("UserId", student.StudentId);
                    return RedirectToAction("Index", "Student"); // student dashboard
                }
            }

            // ❌ Invalid login
            ViewBag.Error = "Invalid username or password.";
            return View();
        }


        // GET: SignUp
        public IActionResult SignUp()
        {
            ViewData["HideNav"] = true; // hide navbar
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string fullName, string email, string password, string confirmPassword)
        {
            ViewData["HideNav"] = true; // keep nav hidden during sign up

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            string studentId = "ucb-" + new Random().Next(1000, 9999);
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

        // ✅ Logout Action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // remove all session values
            return RedirectToAction("Login", "Account"); // go back to login page
        }

        // Password hashing (using SHA256)
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
