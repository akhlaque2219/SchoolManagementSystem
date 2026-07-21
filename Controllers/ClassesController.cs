using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class ClassesController : Controller
    {
        private readonly SchoolContext _context;

        public ClassesController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .Include(c => c.Subjects)
                .OrderBy(c => c.Name).ThenBy(c => c.Section)
                .ToListAsync();
            return View(classes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cls = await _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Students)
                .Include(c => c.Subjects).ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cls == null) return NotFound();
            return View(cls);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateTeachersDropdown();
            return View(new Class());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class cls)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cls);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Class {cls.FullName} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateTeachersDropdown(cls.TeacherId);
            return View(cls);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null) return NotFound();
            await PopulateTeachersDropdown(cls.TeacherId);
            return View(cls);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Class cls)
        {
            if (id != cls.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(cls);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Class {cls.FullName} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateTeachersDropdown(cls.TeacherId);
            return View(cls);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cls = await _context.Classes.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.Id == id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls != null)
            {
                _context.Classes.Remove(cls);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateTeachersDropdown(int? selectedTeacher = null)
        {
            ViewBag.Teachers = new SelectList(await _context.Teachers.Where(t => t.IsActive).ToListAsync(), "Id", "FullName", selectedTeacher);
        }
    }
}
