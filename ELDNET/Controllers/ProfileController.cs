using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using System.Security.Claims;
using ELDNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Make sure this is included for [Authorize]

namespace ELDNET.Controllers
{
    [Authorize] // Uncomment this to ensure only logged-in users can access
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole))
            {
                // This scenario should ideally be caught by [Authorize] if login is properly configured
                return RedirectToAction("Login", "Account");
            }

            ProfileViewModel? profile = null;

            if (userRole == "Student")
            {
                var student = await _context.StudentAccounts
                    .FirstOrDefaultAsync(s => s.StudentId == userIdString);
                if (student != null)
                {
                    profile = new ProfileViewModel
                    {
                        Id = student.Id,
                        UniqueId = student.StudentId,
                        FullName = student.FullName,
                        Email = student.Email,
                        Role = "Student",
                        ProfilePictureUrl = student.ProfilePictureUrl ?? "/images/default-student-profile.png"
                    };
                }
            }
            else if (userRole == "Faculty")
            {
                var faculty = await _context.FacultyAccounts
                    .FirstOrDefaultAsync(f => f.FacultyId == userIdString);
                if (faculty != null)
                {
                    profile = new ProfileViewModel
                    {
                        Id = faculty.Id,
                        UniqueId = faculty.FacultyId,
                        FullName = faculty.FullName,
                        Email = faculty.Email,
                        Role = "Faculty",
                        ProfilePictureUrl = faculty.ProfilePictureUrl ?? "/images/default-faculty-profile.png"
                    };
                }
            }

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // GET: Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "Account");
            }

            ProfileEditViewModel? editModel = null;

            if (userRole == "Student")
            {
                var student = await _context.StudentAccounts
                    .FirstOrDefaultAsync(s => s.StudentId == userIdString);
                if (student != null)
                {
                    editModel = new ProfileEditViewModel
                    {
                        Id = student.Id,
                        UniqueId = student.StudentId,
                        FullName = student.FullName,
                        Email = student.Email,
                        Role = "Student",
                        ProfilePictureUrl = student.ProfilePictureUrl
                    };
                }
            }
            else if (userRole == "Faculty")
            {
                var faculty = await _context.FacultyAccounts
                    .FirstOrDefaultAsync(f => f.FacultyId == userIdString);
                if (faculty != null)
                {
                    editModel = new ProfileEditViewModel
                    {
                        Id = faculty.Id,
                        UniqueId = faculty.FacultyId,
                        FullName = faculty.FullName,
                        Email = faculty.Email,
                        Role = "Faculty",
                        ProfilePictureUrl = faculty.ProfilePictureUrl
                    };
                }
            }

            if (editModel == null)
            {
                return NotFound();
            }

            return View(editModel);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            // Ensure the user trying to edit is the one logged in
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || string.IsNullOrEmpty(userRole) || model.UniqueId != userIdString)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                if (userRole == "Student")
                {
                    var student = await _context.StudentAccounts
                        .FirstOrDefaultAsync(s => s.StudentId == userIdString);

                    if (student == null)
                    {
                        return NotFound();
                    }

                    student.FullName = model.FullName;
                    student.Email = model.Email;


                    _context.StudentAccounts.Update(student);
                }
                else if (userRole == "Faculty")
                {
                    var faculty = await _context.FacultyAccounts
                        .FirstOrDefaultAsync(f => f.FacultyId == userIdString);

                    if (faculty == null)
                    {
                        return NotFound();
                    }

                    faculty.FullName = model.FullName;
                    faculty.Email = model.Email;


                    _context.FacultyAccounts.Update(faculty);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}