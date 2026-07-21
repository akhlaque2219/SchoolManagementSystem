using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public enum FeeStatus
    {
        Pending, Paid, Overdue, Waived, Partial
    }

    public enum FeeCategory
    {
        [Display(Name = "Tuition Fee")] Tuition,
        [Display(Name = "Admission Fee")] Admission,
        [Display(Name = "Library Fee")] Library,
        [Display(Name = "Sports Fee")] Sports,
        [Display(Name = "Transport Fee")] Transport,
        [Display(Name = "Exam Fee")] Exam,
        [Display(Name = "Lab Fee")] Lab,
        [Display(Name = "Miscellaneous")] Miscellaneous
    }

    public class FeeStructure
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fee Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public FeeCategory Category { get; set; }

        [Required, Range(0, 999999)]
        public decimal Amount { get; set; }

        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = DateTime.Today.Year.ToString();

        [Display(Name = "Due Day of Month")]
        [Range(1, 28)]
        public int DueDayOfMonth { get; set; } = 10;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public int? ClassId { get; set; }
        public Class? Class { get; set; }

        public ICollection<FeePayment> Payments { get; set; } = new List<FeePayment>();
    }

    public class FeePayment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Amount Paid")]
        [Range(0, 999999)]
        public decimal AmountPaid { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Month")]
        public string Month { get; set; } = DateTime.Today.ToString("MMMM yyyy");

        [Required]
        public FeeStatus Status { get; set; } = FeeStatus.Pending;

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash";

        [Display(Name = "Transaction ID")]
        public string? TransactionId { get; set; }

        public string? Remarks { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; } = string.Empty;

        // Navigation
        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int FeeStructureId { get; set; }
        public FeeStructure? FeeStructure { get; set; }

        // Computed
        [Display(Name = "Balance")]
        public decimal Balance => (FeeStructure?.Amount ?? 0) - AmountPaid;

        public bool IsLate => Status == FeeStatus.Pending && DateTime.Today > DueDate;
    }
}
