using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private readonly SchoolContext _ctx;
        private readonly UserManager<ApplicationUser> _um;

        public StudentPortalController(SchoolContext ctx, UserManager<ApplicationUser> um)
        {
            _ctx = ctx;
            _um  = um;
        }

        private async Task<Student?> GetStudentAsync()
        {
            var user = await _um.GetUserAsync(User);
            if (user?.StudentId == null) return null;
            return await _ctx.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == user.StudentId);
        }

        // ── Dashboard ────────────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var today = DateTime.Today;
            var grades = await _ctx.Grades.Include(g => g.Subject).Where(g => g.StudentId == student.Id).ToListAsync();
            var attendance = await _ctx.Attendances.Where(a => a.StudentId == student.Id).ToListAsync();
            var books = await _ctx.BookIssues.Include(b => b.Book).Where(b => b.StudentId == student.Id && (b.Status == IssueStatus.Issued || b.Status == IssueStatus.Overdue)).ToListAsync();
            var fees = await _ctx.FeePayments.Include(f => f.FeeStructure).Where(f => f.StudentId == student.Id).ToListAsync();
            var timetable = await _ctx.TimeTables.Include(t => t.Subject).Include(t => t.Teacher)
                .Where(t => t.ClassId == student.ClassId && t.IsActive)
                .OrderBy(t => t.Day).ThenBy(t => t.StartTime).ToListAsync();

            var present = attendance.Count(a => a.Status == AttendanceStatus.Present);
            var total   = attendance.Count;

            ViewBag.Student      = student;
            ViewBag.Grades       = grades;
            ViewBag.AttendanceRate = total > 0 ? Math.Round((double)present / total * 100, 1) : 0;
            ViewBag.BooksIssued  = books;
            ViewBag.PendingFees  = fees.Count(f => f.Status is FeeStatus.Pending or FeeStatus.Overdue or FeeStatus.Partial);
            ViewBag.Timetable    = timetable;
            ViewBag.RecentGrades = grades.OrderByDescending(g => g.ExamDate).Take(5).ToList();

            return View(student);
        }

        // ── My Grades ────────────────────────────────────────────────────
        public async Task<IActionResult> Grades()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var grades = await _ctx.Grades.Include(g => g.Subject)
                .Where(g => g.StudentId == student.Id)
                .OrderByDescending(g => g.ExamDate).ToListAsync();

            ViewBag.Student = student;
            return View(grades);
        }

        // ── My Attendance ────────────────────────────────────────────────
        public async Task<IActionResult> Attendance(int? month)
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            month ??= DateTime.Today.Month;
            var year = DateTime.Today.Year;

            var attendance = await _ctx.Attendances
                .Where(a => a.StudentId == student.Id &&
                            a.Date.Month == month &&
                            a.Date.Year  == year)
                .OrderBy(a => a.Date).ToListAsync();

            ViewBag.Student  = student;
            ViewBag.Month    = month;
            ViewBag.Year     = year;
            ViewBag.Present  = attendance.Count(a => a.Status == AttendanceStatus.Present);
            ViewBag.Absent   = attendance.Count(a => a.Status == AttendanceStatus.Absent);
            ViewBag.Late     = attendance.Count(a => a.Status == AttendanceStatus.Late);
            return View(attendance);
        }

        // ── My Fees ──────────────────────────────────────────────────────
        public async Task<IActionResult> Fees()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var payments = await _ctx.FeePayments.Include(f => f.FeeStructure)
                .Where(f => f.StudentId == student.Id)
                .OrderByDescending(f => f.PaymentDate).ToListAsync();

            ViewBag.Student   = student;
            ViewBag.TotalPaid = payments.Where(p => p.Status == FeeStatus.Paid).Sum(p => p.AmountPaid);
            ViewBag.TotalDue  = payments.Where(p => p.Status is FeeStatus.Pending or FeeStatus.Overdue or FeeStatus.Partial)
                                         .Sum(p => (p.FeeStructure?.Amount ?? 0) - p.AmountPaid);
            return View(payments);
        }

        // ── My Library ───────────────────────────────────────────────────
        public async Task<IActionResult> Library()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var issues = await _ctx.BookIssues.Include(b => b.Book)
                .Where(b => b.StudentId == student.Id)
                .OrderByDescending(b => b.IssueDate).ToListAsync();

            ViewBag.Student = student;
            return View(issues);
        }

        // ── My Timetable ─────────────────────────────────────────────────
        public async Task<IActionResult> TimeTable()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var slots = await _ctx.TimeTables
                .Include(t => t.Subject).Include(t => t.Teacher)
                .Where(t => t.ClassId == student.ClassId && t.IsActive)
                .OrderBy(t => t.Day).ThenBy(t => t.PeriodNo).ToListAsync();

            ViewBag.Student = student;
            ViewBag.Days    = Enum.GetValues<DayOfWeekEnum>();
            return View(slots);
        }

        // ── My Exams ─────────────────────────────────────────────────────
        public async Task<IActionResult> Exams()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var exams = await _ctx.Examinations
                .Include(e => e.Schedules).ThenInclude(s => s.Subject)
                .Where(e => e.ClassId == null || e.ClassId == student.ClassId)
                .OrderByDescending(e => e.StartDate).ToListAsync();

            ViewBag.Student = student;
            return View(exams);
        }

        // ── Report Card ──────────────────────────────────────────────────
        public async Task<IActionResult> ReportCard()
        {
            var student = await GetStudentAsync();
            if (student == null) return Forbid();

            var grades = await _ctx.Grades.Include(g => g.Subject)
                .Where(g => g.StudentId == student.Id)
                .OrderBy(g => g.Subject!.Name).ToListAsync();

            ViewBag.Student = student;
            return View(grades);
        }
    }
}
