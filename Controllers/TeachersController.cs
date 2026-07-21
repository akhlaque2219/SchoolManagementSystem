using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class TeachersController : Controller
    {
        private readonly SchoolContext _context;

        public TeachersController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, bool? isActive)
        {
            var query = _context.Teachers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.FirstName.Contains(search) || t.LastName.Contains(search) || t.TeacherId.Contains(search) || t.Email.Contains(search));

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive);

            ViewBag.Search = search;
            ViewBag.IsActive = isActive;
            return View(await query.OrderBy(t => t.LastName).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Classes).ThenInclude(c => c.Students)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();
            return View(teacher);
        }

        public async Task<IActionResult> Create()
        {
            var teacher = new Teacher { JoiningDate = DateTime.Today };
            teacher.TeacherId = await GenerateTeacherId();
            return View(teacher);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher)
        {
            if (await _context.Teachers.AnyAsync(t => t.Email == teacher.Email))
                ModelState.AddModelError("Email", "Email already exists.");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(teacher.TeacherId))
                    teacher.TeacherId = await GenerateTeacherId();
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Teacher {teacher.FullName} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id) return NotFound();

            if (await _context.Teachers.AnyAsync(t => t.Email == teacher.Email && t.Id != id))
                ModelState.AddModelError("Email", "Email already exists.");

            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Teacher {teacher.FullName} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Teacher deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GenerateTeacherId()
        {
            var count = await _context.Teachers.CountAsync();
            return $"TCH{(count + 1):D3}";
        }
    }
}
