using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    [Route("/Teachers")]
    public class Teachers : Controller
    {
        private readonly ApplicationDbContext _context;

        public Teachers(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getteachers")]
        public async Task<IActionResult> GetTeachers()
        {
            ViewData["Title"] = "Teacher Page";
            var teachers = await _context.Teachers.ToListAsync();
            return View("getTeachers", teachers);
        }
       
        [HttpGet("addteachers")]
        public IActionResult AddTeachers()
        {
            ViewData["Title"] = "Add Teacher";
            return View("addTeachers");
        }

        [HttpPost("addteachers")]
        public async Task<IActionResult> AddTeachers(TeachersModel teacher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(teacher);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Teacher {teacher.FirstName} {teacher.LastName} added successfully!";
                    return RedirectToAction("GetTeachers");
                }
                catch (DbUpdateException ex)
                {
                    // Check if it's a duplicate key violation
                    if (ex.InnerException?.Message.Contains("UNIQUE constraint") == true || 
                        ex.InnerException?.Message.Contains("duplicate") == true)
                    {
                        TempData["ErrorMessage"] = $"Teacher '{teacher.FirstName} {teacher.LastName}' with email '{teacher.Email}' already exists!";
                        ModelState.AddModelError("", "A teacher with this email or name already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while adding the teacher.";
                        ModelState.AddModelError("", "Database error occurred.");
                    }
                }
            }
            return View("addTeachers", teacher);
        }

        [HttpGet("viewteacher/{id}")]
        public async Task<IActionResult> ViewTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Teacher Details";
            return View("viewTeacher", teacher);
        }

        [HttpGet("editteacher/{id}")]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit Teacher";
            return View("editTeacher", teacher);
        }

        [HttpPost("editteacher/{id}")]
        public async Task<IActionResult> EditTeacher(TeachersModel teacher)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                    ViewData["Title"] = "Teacher Updated Successfully";
                    return View("viewTeacher", teacher);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("editTeacher", teacher);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

        [HttpPost("deleteteacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Teacher {teacher.FirstName} {teacher.LastName} deleted successfully!";
            }
            return RedirectToAction("GetTeachers");
        }

        // API Endpoints for Postman/REST API - Returns formatted JSON
        [HttpGet("api/teachers")]
        public async Task<IActionResult> GetAllTeachersApi()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return Json(teachers);
        }

        [HttpGet("api/teachers/{id}")]
        public async Task<IActionResult> GetTeacherByIdApi(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound(new { message = "Teacher not found" });
            }
            return Json(teacher);
        }

        [HttpPost("api/teachers")]
        public async Task<IActionResult> AddTeacherApi([FromBody] TeachersModel teacher)
        {
            if (teacher == null)
            {
                return BadRequest(new { 
                    message = "Teacher data is null", 
                    receivedData = "null",
                    hint = "Check if JSON is properly formatted"
                });
            }
            
            // Check for null or empty properties
            if (string.IsNullOrEmpty(teacher?.FirstName))
            {
                return BadRequest(new { 
                    message = "Teacher first name is required", 
                    receivedData = teacher != null ? "object with null first name" : "null object",
                    hint = "FirstName field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(teacher?.LastName))
            {
                return BadRequest(new { 
                    message = "Teacher last name is required", 
                    receivedData = new {
                        firstName = teacher?.FirstName ?? "null",
                        lastName = teacher?.LastName ?? "null"
                    },
                    hint = "LastName field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(teacher?.Email))
            {
                return BadRequest(new { 
                    message = "Teacher email is required", 
                    receivedData = new {
                        firstName = teacher?.FirstName ?? "null",
                        lastName = teacher?.LastName ?? "null",
                        email = teacher?.Email ?? "null"
                    },
                    hint = "Email field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(teacher?.Subject))
            {
                return BadRequest(new { 
                    message = "Teacher subject is required", 
                    receivedData = new {
                        firstName = teacher?.FirstName ?? "null",
                        lastName = teacher?.LastName ?? "null",
                        subject = teacher?.Subject ?? "null"
                    },
                    hint = "Subject field cannot be null or empty"
                });
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return Json(new { message = "Teacher added successfully", teacher = teacher });
            }
            
            return BadRequest(new { 
                    message = "Validation failed", 
                    receivedData = new {
                        firstName = teacher?.FirstName,
                        lastName = teacher?.LastName,
                        email = teacher?.Email,
                        subject = teacher?.Subject
                    },
                    hint = "Check that all fields are not empty and within length limits"
                });
        }

        [HttpPut("api/teachers/{id}")]
        public async Task<IActionResult> UpdateTeacherApi(int id, [FromBody] TeachersModel teacher)
        {
            if (ModelState.IsValid)
            {
                var existingTeacher = await _context.Teachers.FindAsync(id);
                if (existingTeacher != null)
                {
                    existingTeacher.FirstName = teacher.FirstName;
                    existingTeacher.LastName = teacher.LastName;
                    existingTeacher.Subject = teacher.Subject;
                    existingTeacher.Email = teacher.Email;
                    await _context.SaveChangesAsync();
                    return Json(new { message = "Teacher updated successfully", teacher = existingTeacher });
                }
                return NotFound(new { message = "Teacher not found" });
            }
            return BadRequest(new { message = "Invalid teacher data", receivedData = teacher });
        }

        [HttpDelete("api/teachers/{id}")]
        public async Task<IActionResult> DeleteTeacherApi(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                return Json(new { message = $"Teacher {teacher.FirstName} {teacher.LastName} deleted successfully" });
            }
            return NotFound(new { message = "Teacher not found" });
        }
    }
}
