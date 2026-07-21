using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum ExamType
    {
        [Display(Name = "Mid Term")] MidTerm,
        [Display(Name = "Final Term")] FinalTerm,
        [Display(Name = "Quiz")] Quiz,
        [Display(Name = "Assignment")] Assignment,
        [Display(Name = "Practical")] Practical
    }

    public class Grade
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Exam Type")]
        public ExamType ExamType { get; set; }

        [Required, Range(0, 1000)]
        [Display(Name = "Marks Obtained")]
        public decimal MarksObtained { get; set; }

        [Required, Range(1, 1000)]
        [Display(Name = "Total Marks")]
        public decimal TotalMarks { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; } = DateTime.Today;

        public string? Remarks { get; set; }

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = DateTime.Today.Year.ToString();

        // Navigation
        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        // Computed
        [Display(Name = "Percentage")]
        public decimal Percentage => TotalMarks > 0 ? Math.Round((MarksObtained / TotalMarks) * 100, 2) : 0;

        [Display(Name = "Grade")]
        public string LetterGrade => Percentage switch
        {
            >= 90 => "A+",
            >= 80 => "A",
            >= 70 => "B",
            >= 60 => "C",
            >= 50 => "D",
            _ => "F"
        };

        public bool IsPassed => Percentage >= 40;
    }
}
