using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class GradesController : Controller
    {
        private readonly SchoolContext _context;

        public GradesController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? subjectId, string? examType)
        {
            var query = _context.Grades
                .Include(g => g.Student).ThenInclude(s => s!.Class)
                .Include(g => g.Subject)
                .AsQueryable();

            if (subjectId.HasValue) query = query.Where(g => g.SubjectId == subjectId);
            if (!string.IsNullOrEmpty(examType) && Enum.TryParse<ExamType>(examType, out var et))
                query = query.Where(g => g.ExamType == et);

            ViewBag.Subjects = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", subjectId);
            ViewBag.ExamTypes = new SelectList(Enum.GetNames(typeof(ExamType)));
            ViewBag.SelectedExamType = examType;
            return View(await query.OrderByDescending(g => g.ExamDate).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Grade { ExamDate = DateTime.Today });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError("MarksObtained", "Marks obtained cannot exceed total marks.");

            if (ModelState.IsValid)
            {
                _context.Add(grade);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Grade recorded successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(grade.StudentId, grade.SubjectId);
            return View(grade);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();
            await PopulateDropdowns(grade.StudentId, grade.SubjectId);
            return View(grade);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.Id) return NotFound();
            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError("MarksObtained", "Marks obtained cannot exceed total marks.");

            if (ModelState.IsValid)
            {
                _context.Update(grade);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Grade updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(grade.StudentId, grade.SubjectId);
            return View(grade);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var grade = await _context.Grades
                .Include(g => g.Student).Include(g => g.Subject)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (grade == null) return NotFound();
            return View(grade);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null) { _context.Grades.Remove(grade); await _context.SaveChangesAsync(); TempData["Success"] = "Grade deleted."; }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReportCard(int studentId)
        {
            var student = await _context.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            var grades = await _context.Grades
                .Include(g => g.Subject)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Subject!.Name).ThenBy(g => g.ExamType)
                .ToListAsync();

            ViewBag.Student = student;
            ViewBag.Overall = grades.Count > 0 ? Math.Round(grades.Average(g => g.Percentage), 1) : 0;
            return View(grades);
        }

        private async Task PopulateDropdowns(int? selectedStudent = null, int? selectedSubject = null)
        {
            ViewBag.Students = new SelectList(await _context.Students.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", selectedStudent);
            ViewBag.Subjects = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", selectedSubject);
        }
    }
}
