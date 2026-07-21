using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Data
{
    public class SchoolContext : IdentityDbContext<ApplicationUser>
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookIssue> BookIssues { get; set; }
        public DbSet<TimeTable> TimeTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, FirstName = "John",    LastName = "Smith",    Email = "john.smith@school.edu",    Phone = "555-0101", Qualification = "M.Sc Mathematics",       TeacherId = "TCH001", Specialization = "Mathematics",      JoiningDate = new DateTime(2020,8,15),  Salary = 55000, Gender = "Male",   IsActive = true },
                new Teacher { Id = 2, FirstName = "Sarah",   LastName = "Johnson",  Email = "sarah.johnson@school.edu", Phone = "555-0102", Qualification = "M.A English Literature",  TeacherId = "TCH002", Specialization = "English",          JoiningDate = new DateTime(2019,7,1),   Salary = 52000, Gender = "Female", IsActive = true },
                new Teacher { Id = 3, FirstName = "Michael", LastName = "Davis",    Email = "michael.davis@school.edu", Phone = "555-0103", Qualification = "M.Sc Physics",           TeacherId = "TCH003", Specialization = "Physics & Science", JoiningDate = new DateTime(2021,1,10),  Salary = 57000, Gender = "Male",   IsActive = true },
                new Teacher { Id = 4, FirstName = "Emily",   LastName = "Wilson",   Email = "emily.wilson@school.edu",  Phone = "555-0104", Qualification = "M.Sc Computer Science",  TeacherId = "TCH004", Specialization = "Computer Science",  JoiningDate = new DateTime(2022,3,20),  Salary = 60000, Gender = "Female", IsActive = true }
            );

            modelBuilder.Entity<Class>().HasData(
                new Class { Id = 1, Name = "Grade 9",  Section = "A", AcademicYear = "2025", RoomNumber = "101", MaxStudents = 30, TeacherId = 1 },
                new Class { Id = 2, Name = "Grade 9",  Section = "B", AcademicYear = "2025", RoomNumber = "102", MaxStudents = 30, TeacherId = 2 },
                new Class { Id = 3, Name = "Grade 10", Section = "A", AcademicYear = "2025", RoomNumber = "201", MaxStudents = 35, TeacherId = 3 },
                new Class { Id = 4, Name = "Grade 11", Section = "A", AcademicYear = "2025", RoomNumber = "301", MaxStudents = 30, TeacherId = 4 }
            );

            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = 1, Name = "Mathematics",    Code = "MATH101", CreditHours = 4, TotalMarks = 100, PassMarks = 40, TeacherId = 1, ClassId = 1 },
                new Subject { Id = 2, Name = "English Language",Code = "ENG101", CreditHours = 3, TotalMarks = 100, PassMarks = 40, TeacherId = 2, ClassId = 1 },
                new Subject { Id = 3, Name = "Physics",         Code = "PHY101", CreditHours = 4, TotalMarks = 100, PassMarks = 40, TeacherId = 3, ClassId = 3 },
                new Subject { Id = 4, Name = "Computer Science",Code = "CS101",  CreditHours = 3, TotalMarks = 100, PassMarks = 40, TeacherId = 4, ClassId = 4 },
                new Subject { Id = 5, Name = "Chemistry",       Code = "CHEM101",CreditHours = 4, TotalMarks = 100, PassMarks = 40, TeacherId = 3, ClassId = 3 },
                new Subject { Id = 6, Name = "Biology",         Code = "BIO101", CreditHours = 3, TotalMarks = 100, PassMarks = 40, TeacherId = 3, ClassId = 4 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, FirstName = "Alice", LastName = "Brown",    Email = "alice@student.edu",  Phone = "555-1001", DateOfBirth = new DateTime(2009,5,12),  StudentId = "STU001", Gender = "Female", ClassId = 1, EnrollmentDate = new DateTime(2024,9,1), GuardianName = "Robert Brown",    IsActive = true },
                new Student { Id = 2, FirstName = "Bob",   LastName = "Taylor",   Email = "bob@student.edu",    Phone = "555-1002", DateOfBirth = new DateTime(2009,8,22),  StudentId = "STU002", Gender = "Male",   ClassId = 1, EnrollmentDate = new DateTime(2024,9,1), GuardianName = "James Taylor",    IsActive = true },
                new Student { Id = 3, FirstName = "Carol", LastName = "Anderson", Email = "carol@student.edu",  Phone = "555-1003", DateOfBirth = new DateTime(2009,3,7),   StudentId = "STU003", Gender = "Female", ClassId = 2, EnrollmentDate = new DateTime(2024,9,1), GuardianName = "Linda Anderson",  IsActive = true },
                new Student { Id = 4, FirstName = "David", LastName = "Martinez", Email = "david@student.edu",  Phone = "555-1004", DateOfBirth = new DateTime(2008,11,19), StudentId = "STU004", Gender = "Male",   ClassId = 3, EnrollmentDate = new DateTime(2023,9,1), GuardianName = "Carlos Martinez", IsActive = true },
                new Student { Id = 5, FirstName = "Emma",  LastName = "Garcia",   Email = "emma@student.edu",   Phone = "555-1005", DateOfBirth = new DateTime(2007,6,30),  StudentId = "STU005", Gender = "Female", ClassId = 4, EnrollmentDate = new DateTime(2022,9,1), GuardianName = "Maria Garcia",    IsActive = true },
                new Student { Id = 6, FirstName = "Frank", LastName = "Lee",      Email = "frank@student.edu",  Phone = "555-1006", DateOfBirth = new DateTime(2009,2,14),  StudentId = "STU006", Gender = "Male",   ClassId = 1, EnrollmentDate = new DateTime(2024,9,1), GuardianName = "Kevin Lee",       IsActive = true },
                new Student { Id = 7, FirstName = "Grace", LastName = "White",    Email = "grace@student.edu",  Phone = "555-1007", DateOfBirth = new DateTime(2008,9,3),   StudentId = "STU007", Gender = "Female", ClassId = 3, EnrollmentDate = new DateTime(2023,9,1), GuardianName = "Thomas White",    IsActive = true },
                new Student { Id = 8, FirstName = "Henry", LastName = "Harris",   Email = "henry@student.edu",  Phone = "555-1008", DateOfBirth = new DateTime(2007,12,25), StudentId = "STU008", Gender = "Male",   ClassId = 4, EnrollmentDate = new DateTime(2022,9,1), GuardianName = "Patricia Harris", IsActive = true }
            );

            modelBuilder.Entity<Grade>().HasData(
                new Grade { Id = 1, StudentId = 1, SubjectId = 1, ExamType = ExamType.MidTerm, MarksObtained = 87, TotalMarks = 100, ExamDate = new DateTime(2025,3,15), AcademicYear = "2025" },
                new Grade { Id = 2, StudentId = 1, SubjectId = 2, ExamType = ExamType.MidTerm, MarksObtained = 92, TotalMarks = 100, ExamDate = new DateTime(2025,3,16), AcademicYear = "2025" },
                new Grade { Id = 3, StudentId = 2, SubjectId = 1, ExamType = ExamType.MidTerm, MarksObtained = 75, TotalMarks = 100, ExamDate = new DateTime(2025,3,15), AcademicYear = "2025" },
                new Grade { Id = 4, StudentId = 2, SubjectId = 2, ExamType = ExamType.MidTerm, MarksObtained = 68, TotalMarks = 100, ExamDate = new DateTime(2025,3,16), AcademicYear = "2025" },
                new Grade { Id = 5, StudentId = 4, SubjectId = 3, ExamType = ExamType.MidTerm, MarksObtained = 91, TotalMarks = 100, ExamDate = new DateTime(2025,3,17), AcademicYear = "2025" },
                new Grade { Id = 6, StudentId = 5, SubjectId = 4, ExamType = ExamType.MidTerm, MarksObtained = 95, TotalMarks = 100, ExamDate = new DateTime(2025,3,18), AcademicYear = "2025" }
            );

            modelBuilder.Entity<Attendance>().HasData(
                new Attendance { Id = 1, StudentId = 1, ClassId = 1, Date = DateTime.Today, Status = AttendanceStatus.Present },
                new Attendance { Id = 2, StudentId = 2, ClassId = 1, Date = DateTime.Today, Status = AttendanceStatus.Present },
                new Attendance { Id = 3, StudentId = 3, ClassId = 2, Date = DateTime.Today, Status = AttendanceStatus.Absent  },
                new Attendance { Id = 4, StudentId = 4, ClassId = 3, Date = DateTime.Today, Status = AttendanceStatus.Present },
                new Attendance { Id = 5, StudentId = 5, ClassId = 4, Date = DateTime.Today, Status = AttendanceStatus.Late    },
                new Attendance { Id = 6, StudentId = 6, ClassId = 1, Date = DateTime.Today, Status = AttendanceStatus.Present }
            );

            // Examinations
            modelBuilder.Entity<Examination>().HasData(
                new Examination { Id = 1, Title = "Mid-Term Examination 2025",   ExamType = "Mid Term",   AcademicYear = "2025", StartDate = new DateTime(2025,3,10), EndDate = new DateTime(2025,3,20), Status = ExamStatus.Completed, ClassId = 1, ResultDeclared = true  },
                new Examination { Id = 2, Title = "Final Term Examination 2025", ExamType = "Final Term", AcademicYear = "2025", StartDate = new DateTime(2025,6,1),  EndDate = new DateTime(2025,6,15), Status = ExamStatus.Scheduled, ClassId = 1, ResultDeclared = false },
                new Examination { Id = 3, Title = "Grade 10 Mid-Term 2025",      ExamType = "Mid Term",   AcademicYear = "2025", StartDate = new DateTime(2025,3,10), EndDate = new DateTime(2025,3,20), Status = ExamStatus.Completed, ClassId = 3, ResultDeclared = true  },
                new Examination { Id = 4, Title = "Quiz Week – April",           ExamType = "Quiz",       AcademicYear = "2025", StartDate = new DateTime(2025,4,7),  EndDate = new DateTime(2025,4,11), Status = ExamStatus.Scheduled, ResultDeclared = false }
            );

            modelBuilder.Entity<ExamSchedule>().HasData(
                new ExamSchedule { Id = 1, ExaminationId = 1, SubjectId = 1, ExamDate = new DateTime(2025,3,10), StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(11,0,0), Room = "Hall A",    TotalMarks = 100, PassMarks = 40, TeacherId = 1 },
                new ExamSchedule { Id = 2, ExaminationId = 1, SubjectId = 2, ExamDate = new DateTime(2025,3,12), StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(11,0,0), Room = "Hall A",    TotalMarks = 100, PassMarks = 40, TeacherId = 2 },
                new ExamSchedule { Id = 3, ExaminationId = 2, SubjectId = 1, ExamDate = new DateTime(2025,6,1),  StartTime = new TimeSpan(9,0,0),  EndTime = new TimeSpan(12,0,0), Room = "Main Hall", TotalMarks = 100, PassMarks = 40, TeacherId = 1 },
                new ExamSchedule { Id = 4, ExaminationId = 3, SubjectId = 3, ExamDate = new DateTime(2025,3,11), StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(12,0,0), Room = "Hall B",    TotalMarks = 100, PassMarks = 40, TeacherId = 3 },
                new ExamSchedule { Id = 5, ExaminationId = 3, SubjectId = 5, ExamDate = new DateTime(2025,3,13), StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(12,0,0), Room = "Lab 1",     TotalMarks = 100, PassMarks = 40, TeacherId = 3 }
            );

            // Fee
            modelBuilder.Entity<FeeStructure>().HasData(
                new FeeStructure { Id = 1, Name = "Monthly Tuition – Grade 9",  Category = FeeCategory.Tuition,  Amount = 350, AcademicYear = "2025", DueDayOfMonth = 10, ClassId = 1, IsActive = true },
                new FeeStructure { Id = 2, Name = "Monthly Tuition – Grade 10", Category = FeeCategory.Tuition,  Amount = 400, AcademicYear = "2025", DueDayOfMonth = 10, ClassId = 3, IsActive = true },
                new FeeStructure { Id = 3, Name = "Monthly Tuition – Grade 11", Category = FeeCategory.Tuition,  Amount = 450, AcademicYear = "2025", DueDayOfMonth = 10, ClassId = 4, IsActive = true },
                new FeeStructure { Id = 4, Name = "Annual Library Fee",         Category = FeeCategory.Library,  Amount = 50,  AcademicYear = "2025", DueDayOfMonth = 15, IsActive = true },
                new FeeStructure { Id = 5, Name = "Sports & Activities Fee",    Category = FeeCategory.Sports,   Amount = 80,  AcademicYear = "2025", DueDayOfMonth = 20, IsActive = true },
                new FeeStructure { Id = 6, Name = "Annual Examination Fee",     Category = FeeCategory.Exam,     Amount = 100, AcademicYear = "2025", DueDayOfMonth = 1,  IsActive = true }
            );

            modelBuilder.Entity<FeePayment>().HasData(
                new FeePayment { Id = 1, StudentId = 1, FeeStructureId = 1, AmountPaid = 350, PaymentDate = new DateTime(2025,1,8),  DueDate = new DateTime(2025,1,10), Month = "January 2025",  Status = FeeStatus.Paid,    PaymentMethod = "Online", ReceiptNo = "RCP001", TransactionId = "TXN1001" },
                new FeePayment { Id = 2, StudentId = 1, FeeStructureId = 1, AmountPaid = 350, PaymentDate = new DateTime(2025,2,9),  DueDate = new DateTime(2025,2,10), Month = "February 2025", Status = FeeStatus.Paid,    PaymentMethod = "Online", ReceiptNo = "RCP002", TransactionId = "TXN1002" },
                new FeePayment { Id = 3, StudentId = 1, FeeStructureId = 1, AmountPaid = 350, PaymentDate = new DateTime(2025,3,7),  DueDate = new DateTime(2025,3,10), Month = "March 2025",    Status = FeeStatus.Paid,    PaymentMethod = "Cash",   ReceiptNo = "RCP003" },
                new FeePayment { Id = 4, StudentId = 2, FeeStructureId = 1, AmountPaid = 350, PaymentDate = new DateTime(2025,1,10), DueDate = new DateTime(2025,1,10), Month = "January 2025",  Status = FeeStatus.Paid,    PaymentMethod = "Cheque", ReceiptNo = "RCP004" },
                new FeePayment { Id = 5, StudentId = 2, FeeStructureId = 1, AmountPaid = 0,   PaymentDate = DateTime.Today,          DueDate = new DateTime(2025,2,10), Month = "February 2025", Status = FeeStatus.Overdue, PaymentMethod = "Cash",   ReceiptNo = "RCP005" },
                new FeePayment { Id = 6, StudentId = 4, FeeStructureId = 2, AmountPaid = 400, PaymentDate = new DateTime(2025,1,9),  DueDate = new DateTime(2025,1,10), Month = "January 2025",  Status = FeeStatus.Paid,    PaymentMethod = "Online", ReceiptNo = "RCP006", TransactionId = "TXN2001" },
                new FeePayment { Id = 7, StudentId = 5, FeeStructureId = 3, AmountPaid = 200, PaymentDate = new DateTime(2025,1,12), DueDate = new DateTime(2025,1,10), Month = "January 2025",  Status = FeeStatus.Partial, PaymentMethod = "Cash",   ReceiptNo = "RCP007" },
                new FeePayment { Id = 8, StudentId = 1, FeeStructureId = 4, AmountPaid = 50,  PaymentDate = new DateTime(2025,1,15), DueDate = new DateTime(2025,1,15), Month = "Annual 2025",   Status = FeeStatus.Paid,    PaymentMethod = "Cash",   ReceiptNo = "RCP008" }
            );

            // Library
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1,  Title = "Advanced Mathematics Grade 9",   Author = "R.D. Sharma",         ISBN = "978-81-219-0433-1", Publisher = "Dhanpat Rai",    PublicationYear = 2020, Category = "Mathematics",    ShelfLocation = "A-01", TotalCopies = 10, AvailableCopies = 7, AccessionNo = "ACC001", Status = BookStatus.Available },
                new Book { Id = 2,  Title = "English Grammar & Composition",  Author = "Wren & Martin",       ISBN = "978-81-219-0161-3", Publisher = "S. Chand",       PublicationYear = 2019, Category = "English",        ShelfLocation = "B-01", TotalCopies = 8,  AvailableCopies = 7, AccessionNo = "ACC002", Status = BookStatus.Available },
                new Book { Id = 3,  Title = "Concepts of Physics Vol. 1",     Author = "H.C. Verma",          ISBN = "978-81-771-7459-3", Publisher = "Bharati Bhawan", PublicationYear = 2018, Category = "Physics",        ShelfLocation = "C-01", TotalCopies = 6,  AvailableCopies = 5, AccessionNo = "ACC003", Status = BookStatus.Available },
                new Book { Id = 4,  Title = "Introduction to Algorithms",     Author = "Cormen et al.",       ISBN = "978-02-625-3305-8", Publisher = "MIT Press",      PublicationYear = 2022, Category = "Computer Science",ShelfLocation = "D-01", TotalCopies = 5,  AvailableCopies = 4, AccessionNo = "ACC004", Status = BookStatus.Available },
                new Book { Id = 5,  Title = "Organic Chemistry",              Author = "Paula Y. Bruice",     ISBN = "978-01-345-1304-9", Publisher = "Pearson",        PublicationYear = 2021, Category = "Chemistry",      ShelfLocation = "C-02", TotalCopies = 7,  AvailableCopies = 7, AccessionNo = "ACC005", Status = BookStatus.Available },
                new Book { Id = 6,  Title = "Biology: Life on Earth",         Author = "Audesirk & Audesirk", ISBN = "978-01-358-2039-1", Publisher = "Pearson",        PublicationYear = 2020, Category = "Biology",        ShelfLocation = "E-01", TotalCopies = 6,  AvailableCopies = 6, AccessionNo = "ACC006", Status = BookStatus.Available },
                new Book { Id = 7,  Title = "To Kill a Mockingbird",          Author = "Harper Lee",          ISBN = "978-00-610-8472-1", Publisher = "Lippincott",     PublicationYear = 1960, Category = "Fiction",        ShelfLocation = "F-01", TotalCopies = 4,  AvailableCopies = 2, AccessionNo = "ACC007", Status = BookStatus.Available },
                new Book { Id = 8,  Title = "A Brief History of Time",        Author = "Stephen Hawking",     ISBN = "978-05-530-5340-1", Publisher = "Bantam",         PublicationYear = 1988, Category = "Science",        ShelfLocation = "G-01", TotalCopies = 3,  AvailableCopies = 2, AccessionNo = "ACC008", Status = BookStatus.Available },
                new Book { Id = 9,  Title = "Calculus: Early Transcendentals",Author = "James Stewart",       ISBN = "978-12-852-4152-6", Publisher = "Cengage",        PublicationYear = 2015, Category = "Mathematics",    ShelfLocation = "A-02", TotalCopies = 5,  AvailableCopies = 5, AccessionNo = "ACC009", Status = BookStatus.Available },
                new Book { Id = 10, Title = "The Great Gatsby",               Author = "F. Scott Fitzgerald", ISBN = "978-07-432-7356-5", Publisher = "Scribner",       PublicationYear = 1925, Category = "Fiction",        ShelfLocation = "F-02", TotalCopies = 4,  AvailableCopies = 3, AccessionNo = "ACC010", Status = BookStatus.Available }
            );

            modelBuilder.Entity<BookIssue>().HasData(
                new BookIssue { Id = 1, BookId = 1, StudentId = 1, IssueDate = DateTime.Today.AddDays(-7),  DueDate = DateTime.Today.AddDays(7),   Status = IssueStatus.Issued,   FineAmount = 0,  FinePaid = false },
                new BookIssue { Id = 2, BookId = 3, StudentId = 2, IssueDate = DateTime.Today.AddDays(-10), DueDate = DateTime.Today.AddDays(4),   Status = IssueStatus.Issued,   FineAmount = 0,  FinePaid = false },
                new BookIssue { Id = 3, BookId = 2, StudentId = 3, IssueDate = DateTime.Today.AddDays(-20), DueDate = DateTime.Today.AddDays(-6),  Status = IssueStatus.Overdue,  FineAmount = 12, FinePaid = false },
                new BookIssue { Id = 4, BookId = 4, StudentId = 4, IssueDate = DateTime.Today.AddDays(-5),  DueDate = DateTime.Today.AddDays(9),   Status = IssueStatus.Issued,   FineAmount = 0,  FinePaid = false },
                new BookIssue { Id = 5, BookId = 7, StudentId = 5, IssueDate = DateTime.Today.AddDays(-30), DueDate = DateTime.Today.AddDays(-16), ReturnDate = DateTime.Today.AddDays(-14), Status = IssueStatus.Returned, FineAmount = 4, FinePaid = true }
            );

            // TimeTable
            modelBuilder.Entity<TimeTable>().HasData(
                new TimeTable { Id = 1,  ClassId = 1, Day = DayOfWeekEnum.Monday,    PeriodNo = 1, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 2,  ClassId = 1, Day = DayOfWeekEnum.Monday,    PeriodNo = 2, SubjectId = 2, TeacherId = 2, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 3,  ClassId = 1, Day = DayOfWeekEnum.Monday,    PeriodNo = 3, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(9,45,0), EndTime = new TimeSpan(10,30,0), Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 4,  ClassId = 1, Day = DayOfWeekEnum.Tuesday,   PeriodNo = 1, SubjectId = 2, TeacherId = 2, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 5,  ClassId = 1, Day = DayOfWeekEnum.Tuesday,   PeriodNo = 2, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 6,  ClassId = 1, Day = DayOfWeekEnum.Wednesday, PeriodNo = 1, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 7,  ClassId = 1, Day = DayOfWeekEnum.Wednesday, PeriodNo = 2, SubjectId = 2, TeacherId = 2, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 8,  ClassId = 1, Day = DayOfWeekEnum.Thursday,  PeriodNo = 1, SubjectId = 2, TeacherId = 2, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 9,  ClassId = 1, Day = DayOfWeekEnum.Thursday,  PeriodNo = 2, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 10, ClassId = 1, Day = DayOfWeekEnum.Friday,    PeriodNo = 1, SubjectId = 1, TeacherId = 1, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 11, ClassId = 1, Day = DayOfWeekEnum.Friday,    PeriodNo = 2, SubjectId = 2, TeacherId = 2, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "101",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 12, ClassId = 3, Day = DayOfWeekEnum.Monday,    PeriodNo = 1, SubjectId = 3, TeacherId = 3, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "201",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 13, ClassId = 3, Day = DayOfWeekEnum.Monday,    PeriodNo = 2, SubjectId = 5, TeacherId = 3, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "201",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 14, ClassId = 3, Day = DayOfWeekEnum.Tuesday,   PeriodNo = 1, SubjectId = 5, TeacherId = 3, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "201",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 15, ClassId = 3, Day = DayOfWeekEnum.Wednesday, PeriodNo = 1, SubjectId = 3, TeacherId = 3, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "Lab 1", AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 16, ClassId = 4, Day = DayOfWeekEnum.Monday,    PeriodNo = 1, SubjectId = 4, TeacherId = 4, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "301",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 17, ClassId = 4, Day = DayOfWeekEnum.Monday,    PeriodNo = 2, SubjectId = 6, TeacherId = 3, StartTime = new TimeSpan(8,45,0), EndTime = new TimeSpan(9,30,0),  Room = "301",   AcademicYear = "2025", IsActive = true },
                new TimeTable { Id = 18, ClassId = 4, Day = DayOfWeekEnum.Tuesday,   PeriodNo = 1, SubjectId = 4, TeacherId = 4, StartTime = new TimeSpan(8,0,0),  EndTime = new TimeSpan(8,45,0),  Room = "Lab 2", AcademicYear = "2025", IsActive = true }
            );
        }
    }
}
