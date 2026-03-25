using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    [Route("/Students")]
    public class Students : Controller
    {
        private readonly ApplicationDbContext _context;

        public Students(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getstudents")]
        public async Task<IActionResult> GetStudents()
        {
            ViewData["Title"] = "Student Page";
            var students = await _context.Students.ToListAsync();
            return View("getStudents", students);
        }
       
        [HttpGet("addstudents")]
        public IActionResult AddStudents()
        {
            ViewData["Title"] = "Add Student";
            return View("addStudents");
        }

        [HttpPost("addstudents")]
        public async Task<IActionResult> AddStudents(Studentsmodel student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Student {student.Name} added successfully!";
                    return RedirectToAction("GetStudents");
                }
                catch (DbUpdateException ex)
                {
                    // Check if it's a duplicate key violation
                    if (ex.InnerException?.Message.Contains("UNIQUE constraint") == true || 
                        ex.InnerException?.Message.Contains("duplicate") == true)
                    {
                        TempData["ErrorMessage"] = $"Student '{student.Name}' in batch '{student.batch}' already exists!";
                        ModelState.AddModelError("", "A student with this name and batch already exists.");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while adding the student.";
                        ModelState.AddModelError("", "Database error occurred.");
                    }
                }
            }
            
            ViewData["Title"] = "Add Student";
            return View("addStudents", student);
        }

        [HttpGet("viewstudent/{id}")]
        public async Task<IActionResult> ViewStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Student Details";
            return View("viewStudent", student);
        }

        [HttpGet("editstudent/{id}")]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Edit Student";
            return View("editStudent", student);
        }

        [HttpPost("editstudent/{id}")]
        public async Task<IActionResult> EditStudent(Studentsmodel student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    ViewData["Title"] = "Student Updated Successfully";
                    return View("viewStudent", student);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View("editStudent", student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        [HttpPost("deletestudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Student {student.Name} deleted successfully!";
            }
            return RedirectToAction("GetStudents");
        }

        // API Endpoints for Postman/REST API - Returns formatted JSON
        [HttpGet("api/students")]
        public async Task<IActionResult> GetAllStudentsApi()
        {
            var students = await _context.Students.ToListAsync();
            return Json(students);
        }

        [HttpGet("api/students/{id}")]
        public async Task<IActionResult> GetStudentByIdApi(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }
            return Json(student);
        }

        [HttpPost("api/students")]
        public async Task<IActionResult> AddStudentApi([FromBody] Studentsmodel student)
        {
            Console.WriteLine($"Received student data: Name='{student.Name}', Batch='{student.batch}'");
            
            if (student == null)
            {
                return BadRequest(new { 
                    message = "Student data is null", 
                    receivedData = "null",
                    hint = "Check if JSON is properly formatted"
                });
            }
            
            // Check for null or empty properties
            if (string.IsNullOrEmpty(student?.Name))
            {
                return BadRequest(new { 
                    message = "Student name is required", 
                    receivedData = student != null ? "object with null name" : "null object",
                    hint = "Name field cannot be null or empty"
                });
            }
            
            if (string.IsNullOrEmpty(student?.batch))
            {
                return BadRequest(new { 
                    message = "Student batch is required", 
                    receivedData = new {
                        name = student?.Name ?? "null",
                        batch = student?.batch ?? "null"
                    },
                    hint = "Batch field cannot be null or empty"
                });
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return Json(new { message = "Student added successfully", student = student });
            }
            
            return BadRequest(new { 
                    message = "Validation failed", 
                    receivedData = new {
                        name = student?.Name,
                        batch = student?.batch
                    },
                    hint = "Check that name and batch are not empty and within length limits"
                });
        }

        [HttpPut("api/students/{id}")]
        public async Task<IActionResult> UpdateStudentApi(int id, [FromBody] Studentsmodel student)
        {
            if (ModelState.IsValid)
            {
                var existingStudent = await _context.Students.FindAsync(id);
                if (existingStudent != null)
                {
                    existingStudent.Name = student.Name;
                    existingStudent.batch = student.batch;
                    await _context.SaveChangesAsync();
                    return Json(new { message = "Student updated successfully", student = existingStudent });
                }
                return NotFound(new { message = "Student not found" });
            }
            return BadRequest(new { message = "Invalid student data", errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        [HttpDelete("api/students/{id}")]
        public async Task<IActionResult> DeleteStudentApi(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Json(new { message = $"Student {student.Name} deleted successfully" });
            }
            return NotFound(new { message = "Student not found" });
        }
    }
}
