using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    [Route("/Users")]
    public class Users : Controller
    {
        private readonly ApplicationDbContext _context;

        public Users(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getusers")]
        public async Task<IActionResult> GetUsers()
        {
            ViewData["Title"] = "User Page";
            var users = await _context.Users.ToListAsync();
            return View("getUsers", users);
        }
       
        [HttpGet("addusers")]
        public IActionResult AddUsers()
        {
            ViewData["Title"] = "Add User";
            return View("addUsers");
        }

        [HttpPost("addusers")]
        public async Task<IActionResult> AddUsers(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User {user.FirstName} added successfully!";
                return RedirectToAction("GetUsers");
            }
            return View("addUsers", user);
        }

        [HttpGet("viewuser/{id}")]
        public async Task<IActionResult> ViewUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "User Details";
            return View("viewUser", user);
        }

        [HttpGet("edituser/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit User";
            return View("editUser", user);
        }

        [HttpPost("edituser/{id}")]
        public async Task<IActionResult> EditUser(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    ViewData["Title"] = "User Updated Successfully";
                    return View("viewUser", user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("editUser", user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpPost("deleteuser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User {user.FirstName} deleted successfully!";
            }
            return RedirectToAction("GetUsers");
        }

        // API Endpoints for Postman/REST API - Returns formatted JSON
        [HttpGet("api/users")]
        public async Task<IActionResult> GetAllUsersApi()
        {
            var users = await _context.Users.ToListAsync();
            return Json(users);
        }

        [HttpGet("api/users/{id}")]
        public async Task<IActionResult> GetUserByIdApi(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Json(user);
        }

        [HttpPost("api/createuser")]
        public async Task<IActionResult> AddUserApi([FromBody] UsersModel user)
        {
            if (user == null)
            {
                return BadRequest(new { 
                    message = "User data is null", 
                    receivedData = "null",
                    hint = "Check if JSON is properly formatted"
                });
            }
            
            // Check for null or empty properties
            if (string.IsNullOrEmpty(user?.FirstName))
            {
                return BadRequest(new { 
                    message = "User first name is required", 
                    receivedData = user != null ? "object with null first name" : "null object",
                    hint = "FirstName field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(user?.Username))
            {
                return BadRequest(new { 
                    message = "User username is required", 
                    receivedData = new {
                        firstName = user?.FirstName ?? "null",
                        username = user?.Username ?? "null"
                    },
                    hint = "Username field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(user?.Email))
            {
                return BadRequest(new { 
                    message = "User email is required", 
                    receivedData = new {
                        firstName = user?.FirstName ?? "null",
                        username = user?.Username ?? "null",
                        email = user?.Email ?? "null"
                    },
                    hint = "Email field cannot be null or empty"
                });
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { message = "User added successfully", user = user });
            }
            
            return BadRequest(new { 
                    message = "Validation failed", 
                    receivedData = new {
                        firstName = user?.FirstName,
                        username = user?.Username,
                        email = user?.Email
                    },
                    hint = "Check that all fields are not empty and within length limits"
                });
        }

        [HttpPut("api/updateuser/{id}")]
        public async Task<IActionResult> UpdateUserApi(int id, [FromBody] UsersModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser != null)
                {
                    existingUser.FirstName = user.FirstName;
                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    await _context.SaveChangesAsync();
                    return Json(new { message = "User updated successfully", user = existingUser });
                }
                return NotFound(new { message = "User not found" });
            }
            return BadRequest(new { message = "Invalid user data", receivedData = user });
        }

        [HttpDelete("api/users/{id}")]
        public async Task<IActionResult> DeleteUserApi(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Json(new { message = $"User {user.FirstName} deleted successfully" });
            }
            return NotFound(new { message = "User not found" });
        }
    }
}