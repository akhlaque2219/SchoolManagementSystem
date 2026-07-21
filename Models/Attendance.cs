using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late,
        Excused
    }

    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        public string? Remarks { get; set; }

        // Navigation
        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
