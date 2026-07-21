using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class AttendanceController : Controller
    {
        private readonly SchoolContext _context;

        public AttendanceController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? classId, DateTime? date)
        {
            date ??= DateTime.Today;
            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.Date.Date == date.Value.Date);

            if (classId.HasValue)
                query = query.Where(a => a.ClassId == classId);

            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);
            ViewBag.Date = date.Value.ToString("yyyy-MM-dd");
            return View(await query.OrderBy(a => a.Student!.LastName).ToListAsync());
        }

        public async Task<IActionResult> TakeAttendance(int classId, DateTime? date)
        {
            date ??= DateTime.Today;
            var cls = await _context.Classes.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == classId);
            if (cls == null) return NotFound();

            // Get existing attendance for the day
            var existing = await _context.Attendances
                .Where(a => a.ClassId == classId && a.Date.Date == date.Value.Date)
                .ToListAsync();

            // Build attendance list for all students
            var attendanceList = cls.Students.Select(s => {
                var att = existing.FirstOrDefault(a => a.StudentId == s.Id);
                return att ?? new Attendance { StudentId = s.Id, ClassId = classId, Date = date.Value, Status = AttendanceStatus.Present, Student = s };
            }).ToList();

            ViewBag.Class = cls;
            ViewBag.Date = date.Value.ToString("yyyy-MM-dd");
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);
            return View(attendanceList);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAttendance(int classId, DateTime date, List<int> studentIds, List<string> statuses, List<string?> remarks)
        {
            var existing = await _context.Attendances
                .Where(a => a.ClassId == classId && a.Date.Date == date.Date)
                .ToListAsync();

            _context.Attendances.RemoveRange(existing);

            for (int i = 0; i < studentIds.Count; i++)
            {
                _context.Attendances.Add(new Attendance
                {
                    StudentId = studentIds[i],
                    ClassId = classId,
                    Date = date,
                    Status = Enum.Parse<AttendanceStatus>(statuses[i]),
                    Remarks = remarks.Count > i ? remarks[i] : null
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Attendance saved for {date:MMM dd, yyyy}!";
            return RedirectToAction(nameof(Index), new { classId, date = date.ToString("yyyy-MM-dd") });
        }

        public async Task<IActionResult> StudentReport(int studentId)
        {
            var student = await _context.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            var attendances = await _context.Attendances
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            ViewBag.Student = student;
            ViewBag.Present = attendances.Count(a => a.Status == AttendanceStatus.Present);
            ViewBag.Absent = attendances.Count(a => a.Status == AttendanceStatus.Absent);
            ViewBag.Late = attendances.Count(a => a.Status == AttendanceStatus.Late);
            ViewBag.Total = attendances.Count;
            return View(attendances);
        }
    }
}
