﻿using Microsoft.AspNetCore.Mvc;
using ELDNET.Data;
using ELDNET.Models;
using Microsoft.EntityFrameworkCore;

namespace ELDNET.Controllers
{
    public class StudentAccountsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentAccountsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: StudentAccounts
        public IActionResult Index()
        {
            // Check if user is admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var studentAccounts = _db.StudentAccounts
                .OrderByDescending(s => s.Id)
                .ToList();

            return View(studentAccounts);
        }

        // Optional: GET Details
        public IActionResult Details(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _db.StudentAccounts.Find(id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Optional: POST Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _db.StudentAccounts.Find(id);
            if (student != null)
            {
                _db.StudentAccounts.Remove(student);
                _db.SaveChanges();
                TempData["Message"] = "Student account deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}