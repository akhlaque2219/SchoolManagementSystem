using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.Models.ViewModels;
// Ensure new model enums are accessible

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var totalStudents = await _context.Students.CountAsync();
            var activeStudents = await _context.Students.CountAsync(s => s.IsActive);
            var totalTeachers = await _context.Teachers.CountAsync();
            var activeTeachers = await _context.Teachers.CountAsync(t => t.IsActive);
            var totalClasses = await _context.Classes.CountAsync();
            var totalSubjects = await _context.Subjects.CountAsync();
            var todayAttendance = await _context.Attendances.CountAsync(a => a.Date.Date == today && a.Status == AttendanceStatus.Present);
            var totalTodayMarked = await _context.Attendances.CountAsync(a => a.Date.Date == today);
            double attendanceRate = totalTodayMarked > 0 ? Math.Round((double)todayAttendance / totalTodayMarked * 100, 1) : 0;

            // New module stats
            var upcomingExams = await _context.Examinations.CountAsync(e => e.Status == ExamStatus.Scheduled);
            var overduePayments = await _context.FeePayments.CountAsync(p => p.Status == FeeStatus.Overdue);
            var issuedBooks = await _context.BookIssues.CountAsync(b => b.Status == IssueStatus.Issued || b.Status == IssueStatus.Overdue);
            var totalBooks = await _context.Books.SumAsync(b => b.TotalCopies);

            // Class summaries
            var classes = await _context.Classes
                .Include(c => c.Students)
                .Include(c => c.Teacher)
                .ToListAsync();

            var classSummaries = classes.Select(c => new ClassSummary
            {
                ClassName = c.FullName,
                StudentCount = c.Students.Count,
                MaxStudents = c.MaxStudents,
                Teacher = c.Teacher?.FullName ?? "Unassigned",
                AttendancePercent = 0
            }).ToList();

            // Top students by grades
            var topStudents = await _context.Grades
                .Include(g => g.Student).ThenInclude(s => s!.Class)
                .GroupBy(g => g.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    Average = g.Average(x => (double)x.MarksObtained / (double)x.TotalMarks * 100),
                    Student = g.First().Student
                })
                .OrderByDescending(x => x.Average)
                .Take(5)
                .ToListAsync();

            var topStudentList = topStudents.Select(x => new TopStudent
            {
                Name = x.Student?.FullName ?? "Unknown",
                ClassName = x.Student?.Class?.FullName ?? "N/A",
                AverageScore = Math.Round(x.Average, 1),
                Grade = x.Average switch { >= 90 => "A+", >= 80 => "A", >= 70 => "B", >= 60 => "C", _ => "D" }
            }).ToList();

            var recentActivities = new List<RecentActivity>
            {
                new() { Icon = "fas fa-user-graduate",      Color = "primary", Message = $"{activeStudents} students currently enrolled",           Time = "Today" },
                new() { Icon = "fas fa-chalkboard-teacher", Color = "success", Message = $"{activeTeachers} teachers active this semester",         Time = "Today" },
                new() { Icon = "fas fa-calendar-check",     Color = "info",    Message = $"Attendance rate: {attendanceRate}% today",               Time = today.ToString("MMM dd") },
                new() { Icon = "fas fa-file-alt",           Color = "danger",  Message = $"{upcomingExams} upcoming exam(s) scheduled",             Time = "This term" },
                new() { Icon = "fas fa-dollar-sign",        Color = "warning", Message = $"{overduePayments} overdue fee payment(s)",               Time = "Action needed" },
                new() { Icon = "fas fa-book-open",          Color = "secondary",Message = $"{issuedBooks} books currently issued / {totalBooks} total", Time = "Library" }
            };

            var vm = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalClasses = totalClasses,
                TotalSubjects = totalSubjects,
                ActiveStudents = activeStudents,
                ActiveTeachers = activeTeachers,
                TodayAttendance = todayAttendance,
                AttendanceRate = attendanceRate,
                ClassSummaries = classSummaries,
                TopStudents = topStudentList,
                RecentActivities = recentActivities
            };

            return View(vm);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
