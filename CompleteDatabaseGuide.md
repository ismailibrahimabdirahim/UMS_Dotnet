# UMS Database Integration - Complete Guide

## 🎯 **Project Overview**
University Management System (UMS) with SQL Server database integration using Entity Framework Core.

---

## 🗄️ **Database Connection**

### **Server Information**
- **Server**: `PC\SQLEXPRESS`
- **Database**: `UMS_Db`
- **Authentication**: Windows Authentication
- **Technology**: Entity Framework Core 8.0.0

### **Connection String**
**File**: `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PC\\SQLEXPRESS;Database=UMS_Db;Integrated Security=True;TrustServerCertificate=true;"
  }
}
```

---

## 📊 **Database Schema**

### **Tables Created**
```sql
-- Users Table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    Username NVARCHAR(30) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE
);

-- Students Table
CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    batch NVARCHAR(20) NOT NULL
);

-- Teachers Table
CREATE TABLE Teachers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Subject NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE
);
```

---

## 🔧 **Technical Implementation**

### **Entity Framework Setup**
**File**: `Program.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using UMS.Data;

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### **Database Context**
**File**: `Data/ApplicationDbContext.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using UMS.Models;

namespace UMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<UsersModel> Users { get; set; }
        public DbSet<Studentsmodel> Students { get; set; }
        public DbSet<TeachersModel> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Users table
            modelBuilder.Entity<UsersModel>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            });

            // Configure Students table
            modelBuilder.Entity<Studentsmodel>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.batch).IsRequired().HasMaxLength(20);
            });

            // Configure Teachers table
            modelBuilder.Entity<TeachersModel>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            });
        }
    }
}
```

---

## 💻 **Controller Implementation**

### **Students Controller**
**File**: `Controllers/Students/Students.cs`
```csharp
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

        // GET: Students
        [HttpGet("getstudents")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return View("getStudents", students);
        }

        // POST: Add Student
        [HttpPost("addstudents")]
        public async Task<IActionResult> AddStudents(Studentsmodel student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Student {student.Name} added successfully!";
                return RedirectToAction("GetStudents");
            }
            return View("addStudents", student);
        }

        // POST: Delete Student
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

        // POST: Edit Student
        [HttpPost("editstudent/{id}")]
        public async Task<IActionResult> EditStudent(Studentsmodel student)
        {
            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
                return View("viewStudent", student);
            }
            return View("editStudent", student);
        }
    }
}
```

### **Teachers Controller**
**File**: `Controllers/Teachers/Teachers.cs`
```csharp
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

        // GET: Teachers
        [HttpGet("getteachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await _context.Teachers.ToListAsync();
            return View("getTeachers", teachers);
        }

        // POST: Add Teacher
        [HttpPost("addteachers")]
        public async Task<IActionResult> AddTeachers(TeachersModel teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Teacher {teacher.FirstName} {teacher.LastName} added successfully!";
                return RedirectToAction("GetTeachers");
            }
            return View("addTeachers", teacher);
        }

        // POST: Delete Teacher
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

        // POST: Edit Teacher
        [HttpPost("editteacher/{id}")]
        public async Task<IActionResult> EditTeacher(TeachersModel teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
                return View("viewTeacher", teacher);
            }
            return View("editTeacher", teacher);
        }
    }
}
```

### **Users Controller**
**File**: `Controllers/Users/Users.cs`
```csharp
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

        // GET: Users
        [HttpGet("getusers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View("getUsers", users);
        }

        // POST: Add User
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

        // POST: Delete User
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

        // POST: Edit User
        [HttpPost("edituser/{id}")]
        public async Task<IActionResult> EditUser(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return View("viewUser", user);
            }
            return View("editUser", user);
        }
    }
}
```

---

## 🎨 **Models with Entity Framework Annotations**

### **Users Model**
**File**: `Models/UsersModel.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMS.Models
{
    [Table("Users")]
    public class UsersModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, ErrorMessage = "Username cannot be longer than 30 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string Email { get; set; } = string.Empty;
    }
}
```

### **Students Model**
**File**: `Models/Studentsmodel.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMS.Models
{
    [Table("Students")]
    public class Studentsmodel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Batch is required")]
        [StringLength(20, ErrorMessage = "Batch cannot be longer than 20 characters")]
        public string batch { get; set; } = string.Empty;
    }
}
```

### **Teachers Model**
**File**: `Models/TeachersModel.cs`
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMS.Models
{
    [Table("Teachers")]
    public class TeachersModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100, ErrorMessage = "Subject cannot be longer than 100 characters")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public string Email { get; set; } = string.Empty;
    }
}
```

---

## 🌐 **Home Controller with Database Testing**

### **Home Controller**
**File**: `Controllers/HomeController.cs`
```csharp
using Microsoft.AspNetCore.Mvc;
using UMS.Models;
using UMS.Data;
using Microsoft.EntityFrameworkCore;

namespace UMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Test database connection
            try
            {
                var userCount = _context.Users.CountAsync().Result;
                var studentCount = _context.Students.CountAsync().Result;
                var teacherCount = _context.Teachers.CountAsync().Result;
                
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
    }
}
```

---

## 💾 **Data Operations**

### **CRUD Operations Flow**
1. **User Action** → Web Form
2. **Controller** → C# Method
3. **Entity Framework** → Database Operation
4. **SQL Server** → Data Storage

### **Database Commands**
- **INSERT**: `_context.Add(entity)` + `SaveChangesAsync()`
- **SELECT**: `_context.Table.ToListAsync()`
- **UPDATE**: `_context.Update(entity)` + `SaveChangesAsync()`
- **DELETE**: `_context.Remove(entity)` + `SaveChangesAsync()`

---

## 🔍 **Verification Methods**

### **1. Application Status**
- Home page shows: "✅ Connected to SQL Server!"
- Displays record counts for each table

### **2. SQL Server Verification**
```sql
-- Check data in database
SELECT * FROM Users;
SELECT * FROM Students;
SELECT * FROM Teachers;
```

### **3. Application Logs**
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (27ms) [Parameters=[...]]
      INSERT INTO [Students] ([Name], [batch]) VALUES (@p0, @p1);
```

---

## 📁 **Complete File Structure**

```
UMS_Backup/
├── Data/
│   └── ApplicationDbContext.cs          ✅ Database Context
├── Controllers/
│   ├── HomeController.cs              ✅ Database Testing
│   ├── Students/Students.cs           ✅ Student CRUD
│   ├── Teachers/Teachers.cs           ✅ Teacher CRUD
│   └── Users/Users.cs                 ✅ User CRUD
├── Models/
│   ├── UsersModel.cs                  ✅ User Entity
│   ├── Studentsmodel.cs              ✅ Student Entity
│   └── TeachersModel.cs               ✅ Teacher Entity
├── appsettings.json                   ✅ Connection String
├── Program.cs                         ✅ EF Registration
└── UMS.csproj                         ✅ EF Packages
```

---

## 🚀 **Benefits**

### **✅ Data Persistence**
- Data survives application restarts
- Permanent storage in SQL Server
- No data loss

### **✅ Professional Implementation**
- Industry-standard Entity Framework
- Proper dependency injection
- Asynchronous operations

### **✅ Scalability**
- Handles thousands of records
- Multiple users simultaneously
- SQL Server performance

---

## 🎓 **Key Points for Teacher**

### **Database Connection**
- **Server**: `PC\SQLEXPRESS`
- **Database**: `UMS_Db`
- **Authentication**: Windows Authentication
- **Technology**: Entity Framework Core 8.0.0

### **Implementation**
- All controllers use database operations
- Data persists in SQL Server
- Professional CRUD implementation
- Complete verification system

### **Verification**
- Home page shows connection status
- SQL commands visible in logs
- Data verification with SQL queries

---

## 🌟 **Project Status: COMPLETE**

### **✅ All Requirements Met**
- Database connectivity ✅
- CRUD operations ✅
- Data persistence ✅
- Professional implementation ✅
- Complete documentation ✅

### **🚀 Ready for Demonstration**
- Working database connection
- All operations functional
- Professional code structure
- Complete documentation

---

*This complete guide demonstrates full-stack ASP.NET Core development with professional Entity Framework database integration!*
