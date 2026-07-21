using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Class Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Section { get; set; } = "A";

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = DateTime.Today.Year.ToString();

        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [Range(1, 200)]
        [Display(Name = "Max Students")]
        public int MaxStudents { get; set; } = 30;

        public string? Description { get; set; }

        // Navigation
        public int? TeacherId { get; set; }
        [Display(Name = "Class Teacher")]
        public Teacher? Teacher { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();

        [Display(Name = "Class")]
        public string FullName => $"{Name} - {Section}";

        [Display(Name = "Enrolled Students")]
        public int StudentCount => Students?.Count ?? 0;
    }
}
