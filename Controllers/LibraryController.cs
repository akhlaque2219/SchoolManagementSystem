using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class LibraryController : Controller
    {
        private readonly SchoolContext _context;
        public LibraryController(SchoolContext context) => _context = context;

        public async Task<IActionResult> Index(string? search, string? category)
        {
            var query = _context.Books.AsQueryable();
            if (!string.IsNullOrEmpty(search)) query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search) || b.ISBN!.Contains(search) || b.AccessionNo.Contains(search));
            if (!string.IsNullOrEmpty(category)) query = query.Where(b => b.Category == category);

            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Categories = (await _context.Books.Select(b => b.Category!).Distinct().ToListAsync()).Where(c => c != null).OrderBy(c => c).ToList();
            ViewBag.TotalBooks = await _context.Books.SumAsync(b => b.TotalCopies);
            ViewBag.IssuedBooks = await _context.BookIssues.CountAsync(i => i.Status == IssueStatus.Issued || i.Status == IssueStatus.Overdue);
            ViewBag.OverdueBooks = await _context.BookIssues.CountAsync(i => i.Status == IssueStatus.Overdue);
            return View(await query.OrderBy(b => b.Title).ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books.Include(b => b.Issues).ThenInclude(i => i.Student).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        public IActionResult Create() => View(new Book { TotalCopies = 1, AvailableCopies = 1, AccessionNo = $"ACC{DateTime.Now:yyyyMMddHHmm}" });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (book.AvailableCopies > book.TotalCopies) ModelState.AddModelError("AvailableCopies", "Available copies cannot exceed total copies.");
            if (ModelState.IsValid)
            {
                book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Issued;
                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Book '{book.Title}' added to library!";
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();
            if (book.AvailableCopies > book.TotalCopies) ModelState.AddModelError("AvailableCopies", "Available copies cannot exceed total copies.");
            if (ModelState.IsValid)
            {
                book.Status = book.AvailableCopies > 0 ? BookStatus.Available : BookStatus.Issued;
                _context.Update(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Book updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null) { _context.Remove(book); await _context.SaveChangesAsync(); TempData["Success"] = "Book removed from library."; }
            return RedirectToAction(nameof(Index));
        }

        // ── Issued Books ─────────────────────────────────────────────────

        public async Task<IActionResult> IssuedBooks(string? status)
        {
            var query = _context.BookIssues.Include(i => i.Book).Include(i => i.Student).AsQueryable();
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<IssueStatus>(status, out var st)) query = query.Where(i => i.Status == st);
            ViewBag.StatusFilter = status;
            return View(await query.OrderByDescending(i => i.IssueDate).ToListAsync());
        }

        public async Task<IActionResult> IssueBook(int? bookId)
        {
            await PopulateIssueDropdowns(bookId);
            return View(new BookIssue { IssueDate = DateTime.Today, DueDate = DateTime.Today.AddDays(14), BookId = bookId ?? 0 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueBook(BookIssue issue)
        {
            var book = await _context.Books.FindAsync(issue.BookId);
            if (book == null || book.AvailableCopies <= 0)
                ModelState.AddModelError("BookId", "No copies available for this book.");
            if (await _context.BookIssues.AnyAsync(i => i.StudentId == issue.StudentId && i.BookId == issue.BookId && i.Status == IssueStatus.Issued))
                ModelState.AddModelError("StudentId", "This student already has this book issued.");

            if (ModelState.IsValid)
            {
                issue.Status = IssueStatus.Issued;
                _context.Add(issue);
                book!.AvailableCopies--;
                if (book.AvailableCopies == 0) book.Status = BookStatus.Issued;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Book issued to {(await _context.Students.FindAsync(issue.StudentId))?.FullName}!";
                return RedirectToAction(nameof(IssuedBooks));
            }
            await PopulateIssueDropdowns(issue.BookId);
            return View(issue);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int issueId, decimal fineAmount, bool finePaid)
        {
            var issue = await _context.BookIssues.Include(i => i.Book).FirstOrDefaultAsync(i => i.Id == issueId);
            if (issue == null) return NotFound();

            issue.Status = IssueStatus.Returned;
            issue.ReturnDate = DateTime.Today;
            issue.FineAmount = fineAmount;
            issue.FinePaid = finePaid;

            issue.Book!.AvailableCopies++;
            if (issue.Book.AvailableCopies > 0) issue.Book.Status = BookStatus.Available;

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Book returned successfully! Fine: ${fineAmount}";
            return RedirectToAction(nameof(IssuedBooks));
        }

        private async Task PopulateIssueDropdowns(int? bookId = null, int? studentId = null)
        {
            ViewBag.Books = new SelectList(await _context.Books.Where(b => b.AvailableCopies > 0).ToListAsync(), "Id", "Title", bookId);
            ViewBag.Students = new SelectList(await _context.Students.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", studentId);
        }
    }
}
