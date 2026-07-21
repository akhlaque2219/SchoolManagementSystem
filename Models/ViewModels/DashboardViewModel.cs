namespace SchoolManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }
        public int ActiveStudents { get; set; }
        public int ActiveTeachers { get; set; }
        public int TodayAttendance { get; set; }
        public double AttendanceRate { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public List<ClassSummary> ClassSummaries { get; set; } = new();
        public List<TopStudent> TopStudents { get; set; } = new();
    }

    public class RecentActivity
    {
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class ClassSummary
    {
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public int MaxStudents { get; set; }
        public string Teacher { get; set; } = string.Empty;
        public double AttendancePercent { get; set; }
    }

    public class TopStudent
    {
        public string Name { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public string Grade { get; set; } = string.Empty;
    }
}
