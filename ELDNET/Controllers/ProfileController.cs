using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ELDNET.Data;
using ELDNET.Models;

namespace ELDNET.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            ProfileEditViewModel model = new();

            if (role == "Student")
            {
                var student = await _context.StudentAccounts
                    .FirstOrDefaultAsync(s => s.StudentId == userId);

                if (student == null) return NotFound();

                model = new ProfileEditViewModel
                {
                    Id = student.Id,
                    UniqueId = student.StudentId ?? "",
                    FullName = student.FullName ?? "",
                    Email = student.Email ?? "",
                    Course = student.Course ?? "",
                    Section = student.Section ?? "",
                    Gender = student.Gender ?? "",
                    PhoneNumber = student.PhoneNumber ?? "",
                    Role = "Student",
                    ProfilePictureUrl = string.IsNullOrWhiteSpace(student.ProfilePictureUrl)
                                         ? "/images/defaultprofile.jpg"
                                         : student.ProfilePictureUrl,

                    GatePasses = await _context.GatePasses
                        .Where(g => g.StudentId == userId)
                        .OrderByDescending(g => g.Date)
                        .ToListAsync(),

                    LockerRequests = await _context.LockerRequests
                        .Where(l => l.StudentId == userId)
                        .OrderByDescending(l => l.Date)
                        .ToListAsync(),

                    ReservationRooms = await _context.ReservationRooms
                        .Where(r => r.StudentId == userId)
                        .OrderByDescending(r => r.DateNeeded)
                        .ToListAsync()
                };
            }
            else if (role == "Faculty")
            {
                var faculty = await _context.FacultyAccounts
                    .FirstOrDefaultAsync(f => f.FacultyId == userId);

                if (faculty == null) return NotFound();

                model = new ProfileEditViewModel
                {
                    Id = faculty.Id,
                    UniqueId = faculty.FacultyId ?? "",
                    FullName = faculty.FullName ?? "",
                    Email = faculty.Email ?? "",
                    Gender = faculty.Gender ?? "",
                    PhoneNumber = faculty.PhoneNumber ?? "",
                    Role = "Faculty",
                    ProfilePictureUrl = string.IsNullOrWhiteSpace(faculty.ProfilePictureUrl)
                                         ? "/images/defaultprofile.jpg"
                                         : faculty.ProfilePictureUrl,

                    GatePasses = await _context.GatePasses
                        .Where(g => g.FacultyId == userId)
                        .OrderByDescending(g => g.Date)
                        .ToListAsync(),

                    LockerRequests = await _context.LockerRequests
                        .Where(l => l.FacultyId == userId)
                        .OrderByDescending(l => l.Date)
                        .ToListAsync(),

                    ReservationRooms = await _context.ReservationRooms
                        .Where(r => r.FacultyId == userId)
                        .OrderByDescending(r => r.DateNeeded)
                        .ToListAsync()
                };
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileEditViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (model.UniqueId != userId)
                return Unauthorized();

            string? imagePath = model.ProfilePictureUrl;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads/profiles");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ProfileImage.FileName)}";
                var fullPath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                imagePath = $"/uploads/profiles/{fileName}";
            }

            if (role == "Student")
            {
                var student = await _context.StudentAccounts
                    .FirstOrDefaultAsync(s => s.StudentId == userId);

                if (student == null) return NotFound();

                student.FullName = model.FullName ?? "";
                student.Email = model.Email ?? "";
                student.Course = model.Course ?? "";
                student.Section = model.Section ?? "";
                student.Gender = model.Gender ?? "";
                student.PhoneNumber = model.PhoneNumber ?? "";
                student.ProfilePictureUrl = imagePath;

                _context.StudentAccounts.Update(student);
            }
            else if (role == "Faculty")
            {
                var faculty = await _context.FacultyAccounts
                    .FirstOrDefaultAsync(f => f.FacultyId == userId);

                if (faculty == null) return NotFound();

                faculty.FullName = model.FullName ?? "";
                faculty.Email = model.Email ?? "";
                faculty.Gender = model.Gender ?? "";
                faculty.PhoneNumber = model.PhoneNumber ?? "";
                faculty.ProfilePictureUrl = imagePath;

                _context.FacultyAccounts.Update(faculty);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}