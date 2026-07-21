using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Subject Name")]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(1, 10)]
        [Display(Name = "Credit Hours")]
        public int CreditHours { get; set; } = 3;

        [Range(0, 100)]
        [Display(Name = "Pass Marks")]
        public int PassMarks { get; set; } = 40;

        [Range(0, 100)]
        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; } = 100;

        // Navigation
        public int? TeacherId { get; set; }
        [Display(Name = "Subject Teacher")]
        public Teacher? Teacher { get; set; }

        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
