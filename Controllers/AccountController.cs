using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet("/Account/Login")]
        public IActionResult Login()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginModel());
        }

        // POST: /Account/Login
        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Find user by username first (optimized with index)
                var user = await _context.Users.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null && user.Password == model.Password)
                {
                    // Set session data
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("FirstName", user.FirstName);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());

                    if (model.RememberMe)
                    {
                        // Set cookie to remember user
                        Response.Cookies.Append("Username", user.Username, new CookieOptions 
                        { 
                            Expires = DateTime.Now.AddDays(7) 
                        });
                    }

                    return Json(new { 
                        success = true, 
                        message = "Login successful!", 
                        redirectUrl = Url.Action("Index", "Home"),
                        firstName = user.FirstName
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid username or password!" });
                }
            }

            return Json(new { success = false, message = "Please fill all required fields!" });
        }

        // POST: /Account/Logout
        [HttpPost("/Account/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            return Json(new { success = true, message = "Logged out successfully!", redirectUrl = Url.Action("Login", "Account") });
        }

        // GET: /Account/CheckLogin
        [HttpGet("/Account/CheckLogin")]
        public IActionResult CheckLogin()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { loggedIn = false });
            }
            
            return Json(new { 
                loggedIn = true, 
                username = username,
                firstName = HttpContext.Session.GetString("FirstName")
            });
        }
    }
}
