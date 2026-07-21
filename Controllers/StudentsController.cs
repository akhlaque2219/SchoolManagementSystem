using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? classId, bool? isActive)
        {
            var query = _context.Students.Include(s => s.Class).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.FirstName.Contains(search) || s.LastName.Contains(search) || s.StudentId.Contains(search) || s.Email.Contains(search));

            if (classId.HasValue)
                query = query.Where(s => s.ClassId == classId);

            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive);

            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);
            ViewBag.Search = search;
            ViewBag.IsActive = isActive;

            return View(await query.OrderBy(s => s.LastName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .Include(s => s.Class)
                .Include(s => s.Grades).ThenInclude(g => g.Subject)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            ViewBag.AttendancePresent = student.Attendances.Count(a => a.Status == AttendanceStatus.Present);
            ViewBag.AttendanceTotal = student.Attendances.Count;
            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            var student = new Student { EnrollmentDate = DateTime.Today, DateOfBirth = DateTime.Today.AddYears(-15) };
            student.StudentId = await GenerateStudentId();
            return View(student);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.Email == student.Email))
                ModelState.AddModelError("Email", "Email already exists.");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(student.StudentId))
                    student.StudentId = await GenerateStudentId();
                _context.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Student {student.FullName} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(student.ClassId);
            return View(student);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            await PopulateDropdowns(student.ClassId);
            return View(student);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.Id != id))
                ModelState.AddModelError("Email", "Email already exists.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Student {student.FullName} updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Students.AnyAsync(s => s.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(student.ClassId);
            return View(student);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Student deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? selectedClass = null)
        {
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", selectedClass);
        }

        private async Task<string> GenerateStudentId()
        {
            var count = await _context.Students.CountAsync();
            return $"STU{(count + 1):D3}";
        }
    }
}
