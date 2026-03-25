using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Test database connection
            try
            {
                var userCount = await _context.Users.CountAsync();
                var studentCount = await _context.Students.CountAsync();
                var teacherCount = await _context.Teachers.CountAsync();
                
                ViewBag.DatabaseStatus = "✅ Connected to SQL Server!";
                ViewBag.UserCount = userCount;
                ViewBag.StudentCount = studentCount;
                ViewBag.TeacherCount = teacherCount;
            }
            catch (Exception ex)
            {
                ViewBag.DatabaseStatus = "❌ Database Error: " + ex.Message;
            }
            
            return View();
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
