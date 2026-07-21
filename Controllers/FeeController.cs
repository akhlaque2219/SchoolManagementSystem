using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,Teacher,Staff")]
    public class FeeController : Controller
    {
        private readonly SchoolContext _context;
        public FeeController(SchoolContext context) => _context = context;

        // ── Fee Structure ────────────────────────────────────────────────

        public async Task<IActionResult> Index()
        {
            var structures = await _context.FeeStructures
                .Include(f => f.Class)
                .Include(f => f.Payments)
                .OrderBy(f => f.Category).ThenBy(f => f.Name)
                .ToListAsync();

            var totalCollected = await _context.FeePayments.Where(p => p.Status == FeeStatus.Paid).SumAsync(p => p.AmountPaid);
            var totalPending   = await _context.FeePayments.Where(p => p.Status == FeeStatus.Pending || p.Status == FeeStatus.Overdue).CountAsync();
            var overdueCount   = await _context.FeePayments.Where(p => p.Status == FeeStatus.Overdue).CountAsync();

            ViewBag.TotalCollected = totalCollected;
            ViewBag.TotalPending = totalPending;
            ViewBag.OverdueCount = overdueCount;
            return View(structures);
        }

        public async Task<IActionResult> CreateStructure()
        {
            await PopulateClassDropdown();
            return View(new FeeStructure { AcademicYear = "2025", DueDayOfMonth = 10 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStructure(FeeStructure fs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fs);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Fee structure '{fs.Name}' created!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateClassDropdown(fs.ClassId);
            return View(fs);
        }

        public async Task<IActionResult> EditStructure(int id)
        {
            var fs = await _context.FeeStructures.FindAsync(id);
            if (fs == null) return NotFound();
            await PopulateClassDropdown(fs.ClassId);
            return View(fs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStructure(int id, FeeStructure fs)
        {
            if (id != fs.Id) return NotFound();
            if (ModelState.IsValid) { _context.Update(fs); await _context.SaveChangesAsync(); TempData["Success"] = "Fee structure updated!"; return RedirectToAction(nameof(Index)); }
            await PopulateClassDropdown(fs.ClassId);
            return View(fs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStructure(int id)
        {
            var fs = await _context.FeeStructures.FindAsync(id);
            if (fs != null) { _context.Remove(fs); await _context.SaveChangesAsync(); TempData["Success"] = "Fee structure deleted."; }
            return RedirectToAction(nameof(Index));
        }

        // ── Payments ─────────────────────────────────────────────────────

        public async Task<IActionResult> Payments(int? studentId, string? status, string? month)
        {
            var query = _context.FeePayments
                .Include(p => p.Student).ThenInclude(s => s!.Class)
                .Include(p => p.FeeStructure)
                .AsQueryable();

            if (studentId.HasValue) query = query.Where(p => p.StudentId == studentId);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<FeeStatus>(status, out var st)) query = query.Where(p => p.Status == st);
            if (!string.IsNullOrEmpty(month)) query = query.Where(p => p.Month == month);

            ViewBag.Students = new SelectList(await _context.Students.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", studentId);
            ViewBag.StatusFilter = status;
            ViewBag.MonthFilter = month;
            return View(await query.OrderByDescending(p => p.PaymentDate).ToListAsync());
        }

        public async Task<IActionResult> RecordPayment(int? studentId)
        {
            await PopulatePaymentDropdowns(studentId);
            return View(new FeePayment
            {
                StudentId = studentId ?? 0,
                PaymentDate = DateTime.Today,
                DueDate = DateTime.Today,
                Month = DateTime.Today.ToString("MMMM yyyy"),
                Status = FeeStatus.Paid,
                ReceiptNo = $"RCP{DateTime.Now:yyyyMMddHHmm}"
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(FeePayment payment)
        {
            if (ModelState.IsValid)
            {
                var fs = await _context.FeeStructures.FindAsync(payment.FeeStructureId);
                if (fs != null && payment.AmountPaid >= fs.Amount) payment.Status = FeeStatus.Paid;
                else if (payment.AmountPaid > 0) payment.Status = FeeStatus.Partial;

                _context.Add(payment);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Payment of ${payment.AmountPaid} recorded! Receipt: {payment.ReceiptNo}";
                return RedirectToAction(nameof(Payments));
            }
            await PopulatePaymentDropdowns(payment.StudentId);
            return View(payment);
        }

        public async Task<IActionResult> StudentStatement(int studentId)
        {
            var student = await _context.Students.Include(s => s.Class).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            var payments = await _context.FeePayments
                .Include(p => p.FeeStructure)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            ViewBag.Student = student;
            ViewBag.TotalPaid    = payments.Where(p => p.Status == FeeStatus.Paid).Sum(p => p.AmountPaid);
            ViewBag.TotalPending = payments.Where(p => p.Status is FeeStatus.Pending or FeeStatus.Overdue or FeeStatus.Partial).Sum(p => p.FeeStructure!.Amount - p.AmountPaid);
            return View(payments);
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var payment = await _context.FeePayments
                .Include(p => p.Student).ThenInclude(s => s!.Class)
                .Include(p => p.FeeStructure)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return NotFound();
            return View(payment);
        }

        private async Task PopulateClassDropdown(int? classId = null) =>
            ViewBag.Classes = new SelectList(await _context.Classes.ToListAsync(), "Id", "FullName", classId);

        private async Task PopulatePaymentDropdowns(int? studentId = null, int? feeStructureId = null)
        {
            ViewBag.Students = new SelectList(await _context.Students.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", studentId);
            ViewBag.FeeStructures = new SelectList(await _context.FeeStructures.Where(f => f.IsActive).ToListAsync(), "Id", "Name", feeStructureId);
        }
    }
}
