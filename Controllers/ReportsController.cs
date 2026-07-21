using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.Models.ViewModels;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class ReportsController : Controller
    {
        private readonly SchoolContext _ctx;
        public ReportsController(SchoolContext ctx) => _ctx = ctx;

        // ── Index ────────────────────────────────────────────────────────
        public IActionResult Index() => View();

        // ── Attendance Report ────────────────────────────────────────────
        public async Task<IActionResult> Attendance(int? classId, int? studentId, DateTime? from, DateTime? to)
        {
            from ??= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            to   ??= DateTime.Today;

            var query = _ctx.Attendances
                .Include(a => a.Student).ThenInclude(s => s!.Class)
                .Where(a => a.Date >= from && a.Date <= to)
                .AsQueryable();

            if (classId.HasValue)   query = query.Where(a => a.Student!.ClassId == classId);
            if (studentId.HasValue) query = query.Where(a => a.StudentId == studentId);

            var records = await query.ToListAsync();

            var rows = records
                .GroupBy(a => a.StudentId)
                .Select(g =>
                {
                    var s = g.First().Student!;
                    return new AttendanceReportRow
                    {
                        StudentName = s.FullName,
                        StudentId   = s.StudentId,
                        ClassName   = s.Class?.FullName ?? "—",
                        Present     = g.Count(a => a.Status == AttendanceStatus.Present),
                        Absent      = g.Count(a => a.Status == AttendanceStatus.Absent),
                        Late        = g.Count(a => a.Status == AttendanceStatus.Late),
                        Total       = g.Count()
                    };
                })
                .OrderBy(r => r.ClassName).ThenBy(r => r.StudentName)
                .ToList();

            var vm = new AttendanceReportViewModel
            {
                ClassId        = classId,
                StudentId      = studentId,
                FromDate       = from,
                ToDate         = to,
                Rows           = rows,
                TotalPresent   = rows.Sum(r => r.Present),
                TotalAbsent    = rows.Sum(r => r.Absent),
                TotalLate      = rows.Sum(r => r.Late),
                OverallRate    = rows.Any() ? Math.Round(rows.Average(r => r.Rate), 1) : 0
            };

            await PopulateFilters(classId, studentId);
            return View(vm);
        }

        // ── Fee Report ───────────────────────────────────────────────────
        public async Task<IActionResult> Fees(int? classId, string? status, string? month)
        {
            var query = _ctx.FeePayments
                .Include(p => p.Student).ThenInclude(s => s!.Class)
                .Include(p => p.FeeStructure)
                .AsQueryable();

            if (classId.HasValue) query = query.Where(p => p.Student!.ClassId == classId);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<FeeStatus>(status, out var fs)) query = query.Where(p => p.Status == fs);
            if (!string.IsNullOrEmpty(month))  query = query.Where(p => p.Month == month);

            var records = await query.ToListAsync();

            var rows = records.Select(p => new FeeReportRow
            {
                StudentName = p.Student?.FullName ?? "—",
                ClassName   = p.Student?.Class?.FullName ?? "—",
                FeeName     = p.FeeStructure?.Name ?? "—",
                Month       = p.Month,
                Amount      = p.FeeStructure?.Amount ?? 0,
                Paid        = p.AmountPaid,
                Balance     = p.Balance,
                Status      = p.Status.ToString()
            }).ToList();

            var vm = new FeeReportViewModel
            {
                ClassId        = classId,
                Status         = status,
                Month          = month,
                Rows           = rows,
                TotalExpected  = rows.Sum(r => r.Amount),
                TotalCollected = rows.Sum(r => r.Paid),
                TotalPending   = rows.Sum(r => r.Balance),
                PaidCount      = records.Count(r => r.Status == FeeStatus.Paid),
                OverdueCount   = records.Count(r => r.Status == FeeStatus.Overdue)
            };

            await PopulateFilters(classId);
            ViewBag.StatusFilter = status;
            ViewBag.MonthFilter  = month;
            return View(vm);
        }

        // ── Grades Report ────────────────────────────────────────────────
        public async Task<IActionResult> Grades(int? classId, int? subjectId, string? examType)
        {
            var query = _ctx.Grades
                .Include(g => g.Student).ThenInclude(s => s!.Class)
                .Include(g => g.Subject)
                .AsQueryable();

            if (classId.HasValue)         query = query.Where(g => g.Student!.ClassId == classId);
            if (subjectId.HasValue)       query = query.Where(g => g.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(examType) && Enum.TryParse<ExamType>(examType, out var et))
                query = query.Where(g => g.ExamType == et);

            var records = await query.OrderBy(g => g.Student!.LastName).ToListAsync();

            var rows = records.Select(g => new GradeReportRow
            {
                StudentName = g.Student?.FullName ?? "—",
                StudentId   = g.Student?.StudentId ?? "—",
                ClassName   = g.Student?.Class?.FullName ?? "—",
                SubjectName = g.Subject?.Name ?? "—",
                ExamType    = g.ExamType.ToString(),
                Marks       = g.MarksObtained,
                Total       = g.TotalMarks,
                Percentage  = g.Percentage,
                Grade       = g.LetterGrade,
                Passed      = g.Percentage >= 40
            }).ToList();

            var vm = new GradeReportViewModel
            {
                ClassId      = classId,
                SubjectId    = subjectId,
                ExamType     = examType,
                Rows         = rows,
                ClassAverage = rows.Any() ? Math.Round((double)rows.Average(r => r.Percentage), 1) : 0,
                PassCount    = rows.Count(r => r.Passed),
                FailCount    = rows.Count(r => !r.Passed),
                TotalStudents = rows.Select(r => r.StudentId).Distinct().Count()
            };

            await PopulateFilters(classId);
            ViewBag.Subjects    = new SelectList(await _ctx.Subjects.ToListAsync(), "Id", "Name", subjectId);
            ViewBag.ExamTypes   = Enum.GetNames(typeof(ExamType));
            ViewBag.ExamFilter  = examType;
            return View(vm);
        }

        // ── Library Report ───────────────────────────────────────────────
        public async Task<IActionResult> Library()
        {
            var allBooks  = await _ctx.Books.Include(b => b.Issues).ToListAsync();
            var overdue   = await _ctx.BookIssues
                .Include(i => i.Book).Include(i => i.Student)
                .Where(i => i.Status == IssueStatus.Overdue ||
                            (i.Status == IssueStatus.Issued && DateTime.Today > i.DueDate))
                .OrderBy(i => i.DueDate).ToListAsync();

            var vm = new LibraryReportViewModel
            {
                MostIssuedBooks     = allBooks.OrderByDescending(b => b.Issues.Count).Take(10).ToList(),
                OverdueIssues       = overdue,
                TotalBooks          = allBooks.Count,
                TotalCopies         = allBooks.Sum(b => b.TotalCopies),
                IssuedCopies        = allBooks.Sum(b => b.IssuedCopies),
                TotalFinesCollected = await _ctx.BookIssues.Where(i => i.FinePaid).SumAsync(i => i.FineAmount),
                TotalFinesPending   = await _ctx.BookIssues.Where(i => !i.FinePaid && i.FineAmount > 0).SumAsync(i => i.FineAmount)
            };
            return View(vm);
        }

        // ── Student Summary Report ───────────────────────────────────────
        public async Task<IActionResult> Students(int? classId)
        {
            var query = _ctx.Students.Include(s => s.Class).AsQueryable();
            if (classId.HasValue) query = query.Where(s => s.ClassId == classId);

            var students = await query.OrderBy(s => s.Class!.Name).ThenBy(s => s.LastName).ToListAsync();
            await PopulateFilters(classId);
            return View(students);
        }

        // ── Examination Report ───────────────────────────────────────────
        public async Task<IActionResult> Examinations()
        {
            var exams = await _ctx.Examinations
                .Include(e => e.Class)
                .Include(e => e.Schedules)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
            return View(exams);
        }

        private async Task PopulateFilters(int? classId = null, int? studentId = null)
        {
            ViewBag.Classes  = new SelectList(await _ctx.Classes.ToListAsync(), "Id", "FullName", classId);
            ViewBag.Students = new SelectList(await _ctx.Students.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", studentId);
        }
    }
}
