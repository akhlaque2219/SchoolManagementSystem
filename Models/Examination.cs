using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum ExamStatus
    {
        Scheduled, Ongoing, Completed, Cancelled
    }

    public class Examination
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Exam Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Exam Type")]
        public string ExamType { get; set; } = "Mid Term";

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = DateTime.Today.Year.ToString();

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Status")]
        public ExamStatus Status { get; set; } = ExamStatus.Scheduled;

        public string? Description { get; set; }

        [Display(Name = "Result Declared")]
        public bool ResultDeclared { get; set; } = false;

        // Navigation
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<ExamSchedule> Schedules { get; set; } = new List<ExamSchedule>();

        [Display(Name = "Duration")]
        public string Duration => $"{(EndDate - StartDate).Days + 1} day(s)";
    }

    public class ExamSchedule
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Room / Hall")]
        public string Room { get; set; } = string.Empty;

        [Range(0, 1000)]
        [Display(Name = "Total Marks")]
        public int TotalMarks { get; set; } = 100;

        [Range(0, 1000)]
        [Display(Name = "Pass Marks")]
        public int PassMarks { get; set; } = 40;

        public string? Instructions { get; set; }

        // Navigation
        [Required]
        public int ExaminationId { get; set; }
        public Examination? Examination { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? TeacherId { get; set; }
        [Display(Name = "Invigilator")]
        public Teacher? Teacher { get; set; }

        [Display(Name = "Duration (mins)")]
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

        public string TimeRange => $"{StartTime:hh\\:mm} – {EndTime:hh\\:mm}";
    }
}
