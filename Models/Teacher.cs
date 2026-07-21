using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required]
        public string Qualification { get; set; } = string.Empty;

        [Display(Name = "Joining Date")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        [Display(Name = "Teacher ID")]
        public string TeacherId { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        [Range(0, 100000)]
        public decimal? Salary { get; set; }

        public string Gender { get; set; } = "Male";

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Class> Classes { get; set; } = new List<Class>();

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
