using System.ComponentModel.DataAnnotations;

namespace HospitalEHR.Models
{
    public class AppUser
    {
        [Key] public int UserId { get; set; }

        [Required, StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s\.\-]+$")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(20)]
        [Display(Name = "Employee ID")]
        public string? EmployeeId { get; set; }

        public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt{ get; set; }
        public bool IsActive        { get; set; } = true;

        // Navigation
        public ICollection<Patient>       AssignedPatients { get; set; } = new List<Patient>();
        public ICollection<EhrRecord>     EhrRecords       { get; set; } = new List<EhrRecord>();
        public ICollection<LabOrder>      LabOrders        { get; set; } = new List<LabOrder>();
    }
}
