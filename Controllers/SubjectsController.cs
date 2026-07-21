using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class SubjectsController : Controller
    {
        private readonly SchoolContext _context;

        public SubjectsController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Class)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return View(subjects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Teacher)
                .Include(s => s.Class)
                .Include(s => s.Grades).ThenInclude(g => g.Student)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null) return NotFound();
            return View(subject);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new Subject());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (await _context.Subjects.AnyAsync(s => s.Code == subject.Code))
                ModelState.AddModelError("Code", "Subject code already exists.");

            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Subject {subject.Name} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(subject.TeacherId, subject.ClassId);
            return View(subject);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            await PopulateDropdowns(subject.TeacherId, subject.ClassId);
            return View(subject);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.Id) return NotFound();

            if (await _context.Subjects.AnyAsync(s => s.Code == subject.Code && s.Id != id))
                ModelState.AddModelError("Code", "Subject code already exists.");

            if (ModelState.IsValid)
            {
                _context.Update(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Subject {subject.Name} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns(subject.TeacherId, subject.ClassId);
            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Teacher).Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int? selectedTeacher = null, int? selectedClass = null)
        {
            ViewBag.Teachers = new SelectList(await _context.Teachers.Where(t => t.IsActive).ToListAsync(), "Id", "FullName", selectedTeacher);
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", selectedClass);
        }
    }
}
