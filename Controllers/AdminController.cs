using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.Models.ViewModels;

namespace SchoolManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser>   _userManager;
        private readonly RoleManager<IdentityRole>      _roleManager;
        private readonly SchoolContext                  _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole>    roleManager,
            SchoolContext                context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context     = context;
        }

        // ── Admin Dashboard ──────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var vm = new AdminDashboardViewModel
            {
                TotalUsers    = allUsers.Count,
                ActiveUsers   = allUsers.Count(u => u.IsActive),
                AdminCount    = (await _userManager.GetUsersInRoleAsync("Admin")).Count,
                TeacherCount  = (await _userManager.GetUsersInRoleAsync("Teacher")).Count,
                StaffCount    = (await _userManager.GetUsersInRoleAsync("Staff")).Count,
                StudentCount  = (await _userManager.GetUsersInRoleAsync("Student")).Count,
                RecentUsers   = allUsers.OrderByDescending(u => u.CreatedAt).Take(6).ToList(),
                TotalStudents = await _context.Students.CountAsync(),
                TotalTeachers = await _context.Teachers.CountAsync(),
                TotalClasses  = await _context.Classes.CountAsync(),
            };
            return View(vm);
        }

        // ── Users List ───────────────────────────────────────────────────
        public async Task<IActionResult> Users(string? role, string? search, bool? isActive)
        {
            var allUsers = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                allUsers = allUsers.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search) || u.Email!.Contains(search));
            if (isActive.HasValue)
                allUsers = allUsers.Where(u => u.IsActive == isActive);

            var users = await allUsers.OrderByDescending(u => u.CreatedAt).ToListAsync();

            var vmList = new List<UserViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var primaryRole = roles.FirstOrDefault() ?? "—";
                if (!string.IsNullOrEmpty(role) && primaryRole != role) continue;

                string? linked = null;
                if (u.StudentId.HasValue)
                {
                    var s = await _context.Students.FindAsync(u.StudentId);
                    linked = s != null ? $"Student: {s.FullName}" : null;
                }
                else if (u.TeacherId.HasValue)
                {
                    var t = await _context.Teachers.FindAsync(u.TeacherId);
                    linked = t != null ? $"Teacher: {t.FullName}" : null;
                }

                vmList.Add(new UserViewModel
                {
                    Id            = u.Id,
                    FullName      = u.FullName,
                    Email         = u.Email ?? "",
                    Phone         = u.PhoneNumber,
                    Role          = primaryRole,
                    IsActive      = u.IsActive,
                    CreatedAt     = u.CreatedAt,
                    LastLogin     = u.LastLogin,
                    LinkedEntity  = linked
                });
            }

            ViewBag.RoleFilter = role;
            ViewBag.Search     = search;
            ViewBag.IsActive   = isActive;
            ViewBag.Roles      = IdentitySeed.Roles;
            return View(vmList);
        }

        // ── Edit User ────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);

            var vm = new EditUserViewModel
            {
                Id          = user.Id,
                FirstName   = user.FirstName,
                LastName    = user.LastName,
                Email       = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Role        = roles.FirstOrDefault() ?? "Staff",
                IsActive    = user.IsActive,
                StudentId   = user.StudentId,
                TeacherId   = user.TeacherId
            };
            await PopulateDropdowns(vm.TeacherId, vm.StudentId);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) { await PopulateDropdowns(vm.TeacherId, vm.StudentId); return View(vm); }

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            user.FirstName   = vm.FirstName;
            user.LastName    = vm.LastName;
            user.Email       = vm.Email;
            user.UserName    = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.IsActive    = vm.IsActive;
            user.TeacherId   = vm.TeacherId;
            user.StudentId   = vm.StudentId;
            user.PrimaryRole = vm.Role;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) { foreach (var e in updateResult.Errors) ModelState.AddModelError("", e.Description); await PopulateDropdowns(); return View(vm); }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, vm.Role);

            // Optional password reset
            if (!string.IsNullOrEmpty(vm.NewPassword))
            {
                var token  = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwRes  = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);
                if (!pwRes.Succeeded) { foreach (var e in pwRes.Errors) ModelState.AddModelError("", e.Description); await PopulateDropdowns(); return View(vm); }
            }

            TempData["Success"] = $"User {user.FullName} updated!";
            return RedirectToAction(nameof(Users));
        }

        // ── Toggle Active ────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"User {user.FullName} is now {(user.IsActive ? "active" : "deactivated")}.";
            }
            return RedirectToAction(nameof(Users));
        }

        // ── Delete User ──────────────────────────────────────────────────
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == id) { TempData["Error"] = "You cannot delete your own account."; return RedirectToAction(nameof(Users)); }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null) { await _userManager.DeleteAsync(user); TempData["Success"] = "User deleted."; }
            return RedirectToAction(nameof(Users));
        }

        // ── Settings ─────────────────────────────────────────────────────
        public IActionResult Settings() => View();

        private async Task PopulateDropdowns(int? teacherId = null, int? studentId = null)
        {
            ViewBag.Teachers = new SelectList(await _context.Teachers.ToListAsync(), "Id", "FullName", teacherId);
            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "Id", "FullName", studentId);
            ViewBag.Roles    = IdentitySeed.Roles;
        }
    }

    // AdminDashboard ViewModel (inline)
    public class AdminDashboardViewModel
    {
        public int TotalUsers    { get; set; }
        public int ActiveUsers   { get; set; }
        public int AdminCount    { get; set; }
        public int TeacherCount  { get; set; }
        public int StaffCount    { get; set; }
        public int StudentCount  { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses  { get; set; }
        public List<ApplicationUser> RecentUsers { get; set; } = new();
    }
}
