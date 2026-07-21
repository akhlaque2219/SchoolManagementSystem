using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Display(Name = "Student ID")]
        public string StudentId { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Display(Name = "Guardian Name")]
        public string? GuardianName { get; set; }

        [Display(Name = "Guardian Phone")]
        public string? GuardianPhone { get; set; }

        public string Gender { get; set; } = "Male";

        public bool IsActive { get; set; } = true;

        // Navigation
        public int? ClassId { get; set; }
        public Class? Class { get; set; }
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Age")]
        public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);
    }
}
