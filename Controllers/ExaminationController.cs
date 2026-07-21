using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class ExaminationController : Controller
    {
        private readonly SchoolContext _context;
        public ExaminationController(SchoolContext context) => _context = context;

        public async Task<IActionResult> Index(string? examType, string? status)
        {
            var query = _context.Examinations.Include(e => e.Class).Include(e => e.Schedules).AsQueryable();
            if (!string.IsNullOrEmpty(examType)) query = query.Where(e => e.ExamType == examType);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ExamStatus>(status, out var s)) query = query.Where(e => e.Status == s);
            ViewBag.ExamType = examType;
            ViewBag.StatusFilter = status;
            return View(await query.OrderByDescending(e => e.StartDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var exam = await _context.Examinations
                .Include(e => e.Class)
                .Include(e => e.Schedules).ThenInclude(s => s.Subject)
                .Include(e => e.Schedules).ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null) return NotFound();
            return View(exam);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Examination { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Examination exam)
        {
            if (exam.EndDate < exam.StartDate) ModelState.AddModelError("EndDate", "End date must be after start date.");
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Examination '{exam.Title}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(exam.ClassId);
            return View(exam);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null) return NotFound();
            await PopulateDropdowns(exam.ClassId);
            return View(exam);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Examination exam)
        {
            if (id != exam.Id) return NotFound();
            if (exam.EndDate < exam.StartDate) ModelState.AddModelError("EndDate", "End date must be after start date.");
            if (ModelState.IsValid)
            {
                _context.Update(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Examination updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(exam.ClassId);
            return View(exam);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Examinations.Include(e => e.Class).FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam != null) { _context.Remove(exam); await _context.SaveChangesAsync(); TempData["Success"] = "Examination deleted."; }
            return RedirectToAction(nameof(Index));
        }

        // ── Exam Schedules ──────────────────────────────────────────────

        public async Task<IActionResult> AddSchedule(int examId)
        {
            var exam = await _context.Examinations.Include(e => e.Class).FirstOrDefaultAsync(e => e.Id == examId);
            if (exam == null) return NotFound();
            await PopulateScheduleDropdowns();
            var schedule = new ExamSchedule
            {
                ExaminationId = examId,
                ExamDate = exam.StartDate,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                TotalMarks = 100, PassMarks = 40
            };
            ViewBag.Exam = exam;
            return View(schedule);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchedule(ExamSchedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Exam schedule added!";
                return RedirectToAction(nameof(Details), new { id = schedule.ExaminationId });
            }
            var exam = await _context.Examinations.FindAsync(schedule.ExaminationId);
            ViewBag.Exam = exam;
            await PopulateScheduleDropdowns(schedule.SubjectId, schedule.TeacherId);
            return View(schedule);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.ExamSchedules.FindAsync(id);
            if (schedule != null) { _context.Remove(schedule); await _context.SaveChangesAsync(); TempData["Success"] = "Schedule removed."; return RedirectToAction(nameof(Details), new { id = schedule.ExaminationId }); }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? classId = null)
        {
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);
            ViewBag.ExamTypes = new[] { "Mid Term", "Final Term", "Quiz", "Assignment", "Unit Test", "Annual" };
        }
        private async Task PopulateScheduleDropdowns(int? subjectId = null, int? teacherId = null)
        {
            ViewBag.Subjects = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", subjectId);
            ViewBag.Teachers = new SelectList(await _context.Teachers.Where(t => t.IsActive).ToListAsync(), "Id", "FullName", teacherId);
        }
    }
}
