using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models.ViewModels
{
    // ── Login ────────────────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    // ── Register ─────────────────────────────────────────────────────────
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Staff";

        [Required, DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Optional link to existing teacher record
        public int? TeacherId { get; set; }
    }

    // ── User Management ──────────────────────────────────────────────────
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? LinkedEntity { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password (leave blank to keep current)")]
        public string? NewPassword { get; set; }
    }

    // ── Change Password ──────────────────────────────────────────────────
    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ── Reports ViewModels ───────────────────────────────────────────────
    public class AttendanceReportViewModel
    {
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<AttendanceReportRow> Rows { get; set; } = new();
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public double OverallRate { get; set; }
    }

    public class AttendanceReportRow
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Total { get; set; }
        public double Rate => Total > 0 ? Math.Round((double)Present / Total * 100, 1) : 0;
    }

    public class FeeReportViewModel
    {
        public string? Month { get; set; }
        public string? Status { get; set; }
        public int? ClassId { get; set; }

        public List<FeeReportRow> Rows { get; set; } = new();
        public decimal TotalExpected { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalPending { get; set; }
        public int PaidCount { get; set; }
        public int OverdueCount { get; set; }
    }

    public class FeeReportRow
    {
        public string StudentName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string FeeName { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class GradeReportViewModel
    {
        public int? ClassId { get; set; }
        public int? SubjectId { get; set; }
        public string? ExamType { get; set; }

        public List<GradeReportRow> Rows { get; set; } = new();
        public double ClassAverage { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public int TotalStudents { get; set; }
    }

    public class GradeReportRow
    {
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public decimal Marks { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = string.Empty;
        public bool Passed { get; set; }
    }

    public class LibraryReportViewModel
    {
        public List<Book> MostIssuedBooks { get; set; } = new();
        public List<BookIssue> OverdueIssues { get; set; } = new();
        public int TotalBooks { get; set; }
        public int TotalCopies { get; set; }
        public int IssuedCopies { get; set; }
        public decimal TotalFinesCollected { get; set; }
        public decimal TotalFinesPending { get; set; }
    }
}
