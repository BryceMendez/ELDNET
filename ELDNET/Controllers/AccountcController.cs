using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            ViewData["HideNav"] = true;
            if (username == "admin" && password == "admin123")
            {
                // For now, keeping your existing Admin session logic
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", "admin");
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "admin"),
                    new Claim(ClaimTypes.Name, "Administrator"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var adminClaimsIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(adminClaimsIdentity), new AuthenticationProperties { IsPersistent = rememberMe });

                return RedirectToAction("Index", "Approval");
            }
            var admin = _db.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserId", admin.Username);
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.Username),
                    new Claim(ClaimTypes.Name, admin.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var adminClaimsIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(adminClaimsIdentity), new AuthenticationProperties { IsPersistent = rememberMe });

                return RedirectToAction("Index", "Approval");
            }

            // Check for student login
            var student = _db.StudentAccounts
                .FirstOrDefault(s => s.StudentId == username || s.Email == username);
            if (student != null)
            {
                string hashedPassword = HashPassword(password);
                if (student.PasswordHash == hashedPassword)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                        new Claim(ClaimTypes.Name, student.FullName),     
                        new Claim(ClaimTypes.Email, student.Email),  
                        new Claim(ClaimTypes.Role, "Student")        
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe,

                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    // --------------------------------------------------------
                    HttpContext.Session.SetString("UserRole", "Student");
                    HttpContext.Session.SetString("UserId", student.StudentId);
                    HttpContext.Session.SetString("FullName", student.FullName);

                    return RedirectToAction("Index", "Home");
                }
            }

            var faculty = _db.FacultyAccounts
                .FirstOrDefault(f => f.FacultyId == username || f.Email == username);
            if (faculty != null)
            {
                string hashedPassword = HashPassword(password);
                if (faculty.PasswordHash == hashedPassword)
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, faculty.FacultyId),
                        new Claim(ClaimTypes.Name, faculty.FullName),
                        new Claim(ClaimTypes.Email, faculty.Email),
                        new Claim(ClaimTypes.Role, "Faculty")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = rememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    // --------------------------------------------------------

                    // Your custom session variables (optional)
                    HttpContext.Session.SetString("UserRole", "Faculty");
                    HttpContext.Session.SetString("UserId", faculty.FacultyId);
                    HttpContext.Session.SetString("FullName", faculty.FullName);

                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["Error"] = "Invalid username or password.";
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear(); // Clears your custom session data
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult SignUp()
        {
            ViewData["HideNav"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string fullName, string email, string password, string confirmPassword, string accountType)
        {
            ViewData["HideNav"] = true;

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (_db.StudentAccounts.Any(sa => sa.Email == email) || _db.FacultyAccounts.Any(fa => fa.Email == email))
            {
                ViewBag.Error = "An account with this email already exists.";
                return View();
            }

            string hashedPassword = HashPassword(password);
            string generatedId = "";
            string userRoleDisplayName = "";

            if (accountType == "Student")
            {
                userRoleDisplayName = "Student";
                do
                {
                    generatedId = "ucb-" + new Random().Next(100000, 999999).ToString();
                }
                while (_db.StudentAccounts.Any(sa => sa.StudentId == generatedId));

                var studentAccount = new StudentAccount
                {
                    StudentId = generatedId,
                    FullName = fullName,
                    Email = email,
                    PasswordHash = hashedPassword
                };
                _db.StudentAccounts.Add(studentAccount);
            }
            else if (accountType == "Faculty")
            {
                userRoleDisplayName = "Faculty";
                do
                {
                    generatedId = "ucbf-" + new Random().Next(100000, 999999).ToString();
                }
                while (_db.FacultyAccounts.Any(fa => fa.FacultyId == generatedId));

                var facultyAccount = new FacultyAccount
                {
                    FacultyId = generatedId,
                    FullName = fullName,
                    Email = email,
                    PasswordHash = hashedPassword
                };
                _db.FacultyAccounts.Add(facultyAccount);
            }
            else
            {
                ViewBag.Error = "Invalid account type selected.";
                return View();
            }

            _db.SaveChanges();

            TempData["Message"] = $"Account created! Your {userRoleDisplayName} ID is {generatedId}. Use it to log in.";
            return RedirectToAction("Login");
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