# 🎓 UMS Role-Based Login Guide

## 🔐 **Login Credentials - Role-Based Access**

### **👑 ADMIN ACCESS**
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: Admin
- **Access**: Full system control + Red badge
- **Features**: All management options, user management, system settings

### **👨‍🏫 TEACHER ACCESS**
- **Username**: `teacher1`
- **Password**: `teach123`
- **Role**: Teacher
- **Access**: Student & class management + Blue badge
- **Features**: Manage students, view classes, teacher dashboard

- **Username**: `teacher2`
- **Password**: `teach123`
- **Role**: Teacher
- **Access**: Same as teacher1

### **👨‍🎓 STUDENT ACCESS**
- **Username**: `student1`
- **Password**: `stud123`
- **Role**: Student
- **Access**: Personal data viewing + Green badge
- **Features**: View own records, class schedule, grades

- **Username**: `student2`
- **Password**: `stud123`
- **Role**: Student
- **Access**: Same as student1

### **👤 REGULAR USER ACCESS**
- **Username**: `user1`
- **Password**: `user123`
- **Role**: User
- **Access**: Basic features + Gray badge
- **Features**: Limited viewing, basic navigation

---

## 🎯 **How to Test Role-Based System**

### **Step 1: Add Users to Database**
```sql
-- Run this in SQL Server Management Studio
ALTER TABLE Users ADD Password NVARCHAR(100) NOT NULL DEFAULT '';
ALTER TABLE Users ADD Role NVARCHAR(50) NOT NULL DEFAULT 'User';

INSERT INTO Users (FirstName, Username, Email, Password, Role) VALUES
('System Admin', 'admin', 'admin@ums.com', 'admin123', 'Admin'),
('Ahmed Teacher', 'teacher1', 'ahmed@ums.com', 'teach123', 'Teacher'),
('Fatima Teacher', 'teacher2', 'fatima@ums.com', 'teach123', 'Teacher'),
('Mohamed Student', 'student1', 'mohamed@ums.com', 'stud123', 'Student'),
('Aisha Student', 'student2', 'aisha@ums.com', 'stud123', 'Student'),
('Regular User', 'user1', 'user@ums.com', 'user123', 'User');
```

### **Step 2: Run Application**
```bash
dotnet watch run
```

### **Step 3: Test Different Logins**
1. Go to `http://localhost:5298`
2. Try different usernames/passwords above
3. See different interfaces based on role!

---

## 🌟 **What You'll See**

### **🔴 Admin Experience**
- Red "Admin" badge in navigation
- Full dashboard access
- All management buttons visible
- System administration features

### **🔵 Teacher Experience**
- Blue "Teacher" badge
- Student management tools
- Class management features
- Teacher dashboard

### **🟢 Student Experience**
- Green "Student" badge
- Personal data view
- Class schedule
- Grade viewing

### **⚫ User Experience**
- Gray "User" badge
- Basic navigation
- Limited features
- Standard access

---

## 🎨 **Visual Role Indicators**

| Role | Badge Color | Icon | Access Level |
|------|-------------|------|-------------|
| Admin | 🔴 Red | 👑 | Full Access |
| Teacher | 🔵 Blue | 👨‍🏫 | Teacher Access |
| Student | 🟢 Green | 👨‍🎓 | Student Access |
| User | ⚫ Gray | 👤 | Basic Access |

---

## 🚀 **Quick Testing Steps**

1. **Login as Admin** → See red badge + full access
2. **Logout** → Return to login page
3. **Login as Teacher** → See blue badge + teacher features
4. **Logout** → Return to login page
5. **Login as Student** → See green badge + student view
6. **Logout** → Return to login page
7. **Login as User** → See gray badge + basic access

---

## ✅ **System Features**

- 🔐 **Real Password Validation** - No demo passwords
- 🎭 **Role-Based UI** - Different interfaces per role
- 🏷️ **Role Badges** - Color-coded identification
- 🔄 **Session Management** - Secure login sessions
- 📱 **Responsive Design** - Works on all devices
- 🎨 **Beautiful Interface** - Modern, clean design

---

**🎓 Your UMS now has professional role-based authentication! Test all the different logins above!**
