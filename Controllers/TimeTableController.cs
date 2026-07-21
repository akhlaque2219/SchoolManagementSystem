using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class TimeTableController : Controller
    {
        private readonly SchoolContext _context;
        public TimeTableController(SchoolContext context) => _context = context;

        public async Task<IActionResult> Index(int? classId)
        {
            classId ??= (await _context.Classes.FirstOrDefaultAsync())?.Id;
            var classes = await _context.Classes.OrderBy(c => c.Name).ThenBy(c => c.Section).ToListAsync();

            List<TimeTable> slots = new();
            if (classId.HasValue)
            {
                slots = await _context.TimeTables
                    .Include(t => t.Subject)
                    .Include(t => t.Teacher)
                    .Include(t => t.Class)
                    .Where(t => t.ClassId == classId && t.IsActive)
                    .OrderBy(t => t.Day).ThenBy(t => t.PeriodNo)
                    .ToListAsync();
            }

            ViewBag.Classes = new SelectList(classes, "Id", "FullName", classId);
            ViewBag.SelectedClassId = classId;
            ViewBag.Days = Enum.GetValues<DayOfWeekEnum>();
            return View(slots);
        }

        public async Task<IActionResult> Create(int? classId)
        {
            await PopulateDropdowns(classId);
            return View(new TimeTable
            {
                ClassId = classId ?? 0,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(8, 45, 0),
                AcademicYear = "2025",
                PeriodNo = 1,
                IsActive = true
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TimeTable tt)
        {
            if (tt.EndTime <= tt.StartTime) ModelState.AddModelError("EndTime", "End time must be after start time.");
            if (await _context.TimeTables.AnyAsync(t => t.ClassId == tt.ClassId && t.Day == tt.Day && t.PeriodNo == tt.PeriodNo && t.IsActive))
                ModelState.AddModelError("PeriodNo", "This period already exists for this class on this day.");

            if (ModelState.IsValid)
            {
                _context.Add(tt);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period added to timetable!";
                return RedirectToAction(nameof(Index), new { classId = tt.ClassId });
            }
            await PopulateDropdowns(tt.ClassId, tt.SubjectId, tt.TeacherId);
            return View(tt);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tt = await _context.TimeTables.FindAsync(id);
            if (tt == null) return NotFound();
            await PopulateDropdowns(tt.ClassId, tt.SubjectId, tt.TeacherId);
            return View(tt);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TimeTable tt)
        {
            if (id != tt.Id) return NotFound();
            if (tt.EndTime <= tt.StartTime) ModelState.AddModelError("EndTime", "End time must be after start time.");
            if (ModelState.IsValid)
            {
                _context.Update(tt);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period updated!";
                return RedirectToAction(nameof(Index), new { classId = tt.ClassId });
            }
            await PopulateDropdowns(tt.ClassId, tt.SubjectId, tt.TeacherId);
            return View(tt);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tt = await _context.TimeTables.FindAsync(id);
            if (tt != null)
            {
                var classId = tt.ClassId;
                _context.Remove(tt);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Period removed.";
                return RedirectToAction(nameof(Index), new { classId });
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TeacherSchedule(int teacherId)
        {
            var teacher = await _context.Teachers.FindAsync(teacherId);
            if (teacher == null) return NotFound();

            var slots = await _context.TimeTables
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Where(t => t.TeacherId == teacherId && t.IsActive)
                .OrderBy(t => t.Day).ThenBy(t => t.StartTime)
                .ToListAsync();

            ViewBag.Teacher = teacher;
            ViewBag.Teachers = new SelectList(await _context.Teachers.Where(t => t.IsActive).ToListAsync(), "Id", "FullName", teacherId);
            ViewBag.Days = Enum.GetValues<DayOfWeekEnum>();
            return View(slots);
        }

        private async Task PopulateDropdowns(int? classId = null, int? subjectId = null, int? teacherId = null)
        {
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);
            ViewBag.Subjects = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", subjectId);
            ViewBag.Teachers = new SelectList(await _context.Teachers.Where(t => t.IsActive).ToListAsync(), "Id", "FullName", teacherId);
            ViewBag.Days = Enum.GetValues<DayOfWeekEnum>();
        }
    }
}
