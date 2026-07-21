using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum DayOfWeekEnum
    {
        Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday
    }

    public class TimeTable
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Day")]
        public DayOfWeekEnum Day { get; set; }

        [Required]
        [Display(Name = "Period No")]
        [Range(1, 12)]
        public int PeriodNo { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Room")]
        public string? Room { get; set; }

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = DateTime.Today.Year.ToString();

        public bool IsActive { get; set; } = true;

        // Navigation
        [Required]
        public int ClassId { get; set; }
        public Class? Class { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        // Computed
        public string TimeRange => $"{StartTime:hh\\:mm} – {EndTime:hh\\:mm}";

        [Display(Name = "Duration (mins)")]
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
    }
}
