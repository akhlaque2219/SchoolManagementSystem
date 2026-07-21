using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum BookStatus
    {
        Available, Issued, Reserved, Lost, Damaged
    }

    public enum IssueStatus
    {
        Issued, Returned, Overdue, Lost
    }

    public class Book
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        public string? Publisher { get; set; }

        [Display(Name = "Publication Year")]
        public int? PublicationYear { get; set; }

        public string? Category { get; set; }

        [Display(Name = "Shelf Location")]
        public string? ShelfLocation { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; } = 1;

        [Range(0, 10000)]
        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; } = 1;

        [Display(Name = "Book Status")]
        public BookStatus Status { get; set; } = BookStatus.Available;

        [Range(0, 99999)]
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Accession No")]
        public string AccessionNo { get; set; } = string.Empty;

        public ICollection<BookIssue> Issues { get; set; } = new List<BookIssue>();

        [Display(Name = "Issued Copies")]
        public int IssuedCopies => TotalCopies - AvailableCopies;
    }

    public class BookIssue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Required]
        public IssueStatus Status { get; set; } = IssueStatus.Issued;

        public string? Remarks { get; set; }

        [Display(Name = "Fine Amount")]
        [Range(0, 9999)]
        public decimal FineAmount { get; set; } = 0;

        [Display(Name = "Fine Paid")]
        public bool FinePaid { get; set; } = false;

        // Navigation
        [Required]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Computed
        public bool IsOverdue => Status == IssueStatus.Issued && DateTime.Today > DueDate;

        [Display(Name = "Days Overdue")]
        public int DaysOverdue => IsOverdue ? (int)(DateTime.Today - DueDate).TotalDays : 0;

        [Display(Name = "Calculated Fine")]
        public decimal CalculatedFine => DaysOverdue * 2.00m; // $2 per day
    }
}
