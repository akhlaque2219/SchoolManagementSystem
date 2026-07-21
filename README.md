# 🎓 Greenwood Academy — School Management System

A **complete, production-ready** School Management System built with **ASP.NET Core 8 MVC** + **Entity Framework Core** + **SQLite**.

---

## 🚀 Features

| Module | Features |
|--------|----------|
| **Dashboard** | Live stats, class overview, top students, quick actions |
| **Students** | Full CRUD, search/filter, profile cards, report card |
| **Teachers** | Faculty management with card view, subjects & classes |
| **Classes** | Section management, capacity tracking, student roster |
| **Subjects** | Curriculum management, teacher & class assignment |
| **Attendance** | Bulk attendance marking, student reports, status tracking |
| **Grades** | Multi-exam-type grades, automatic letter grades, report cards |

---

## 🛠️ Tech Stack

- **ASP.NET Core 8 MVC**
- **Entity Framework Core 8** with SQLite
- **Bootstrap 5.3** (responsive, mobile-first)
- **Font Awesome 6** icons
- **Chart.js** for charts

---

## ⚡ Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run the App

```bash
# 1. Navigate to project folder
cd SchoolManagement

# 2. Restore packages
dotnet restore

# 3. Run (database auto-creates with seed data)
dotnet run
```

Then open: **https://localhost:5001** or **http://localhost:5000**

---

## 📁 Project Structure

```
SchoolManagement/
├── Controllers/         # MVC Controllers
│   ├── HomeController.cs      # Dashboard
│   ├── StudentsController.cs
│   ├── TeachersController.cs
│   ├── ClassesController.cs
│   ├── SubjectsController.cs
│   ├── AttendanceController.cs
│   └── GradesController.cs
├── Models/              # Domain Models
│   ├── Student.cs
│   ├── Teacher.cs
│   ├── Class.cs
│   ├── Subject.cs
│   ├── Attendance.cs
│   ├── Grade.cs
│   └── ViewModels/
├── Data/
│   └── SchoolContext.cs  # EF DbContext + Seed Data
├── Views/               # Razor Views
└── wwwroot/             # Static assets
```

---

## 🌱 Seed Data

The database auto-seeds with:
- **4 teachers** (Mathematics, English, Physics, CS)
- **4 classes** (Grade 9A, 9B, 10A, 11A)
- **6 subjects**
- **8 students** across different classes
- **6 grade records**
- **6 attendance records** (today)

---

## 📦 Migrations (if needed)

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🎨 Customization

- **School Name**: Edit in `appsettings.json` → `AppSettings.SchoolName`
- **Colors**: Modify `wwwroot/css/site.css` CSS variables
- **Database**: Change connection string in `appsettings.json` (supports SQL Server too)

---

## 🔧 Switch to SQL Server

In `appsettings.json`:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SchoolDB;Trusted_Connection=true;"
```

In `Program.cs`, replace `UseSqlite` with:
```csharp
options.UseSqlServer(connectionString)
```

---

*Built with ❤️ — Greenwood Academy SMS*
