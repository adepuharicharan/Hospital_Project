using HospitalEHR.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalEHR.ViewModels
{
    public class CreateUserVM
    {
        [Required, StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s\.\-]+$", ErrorMessage = "Letters, spaces, dots and hyphens only")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Must have uppercase, lowercase, digit and special character")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a role")]
        public UserRole Role { get; set; }

        [StringLength(100)]
        [Display(Name = "Specialization (for Doctors)")]
        public string? Specialization { get; set; }

        [StringLength(20)]
        [Display(Name = "Employee ID")]
        public string? EmployeeId { get; set; }
    }

    public class AdminDashVM
    {
        public int TotalPatients        { get; set; }
        public int AdmittedCount        { get; set; }
        public int TotalDoctors         { get; set; }
        public int TotalUsers           { get; set; }
        public decimal TotalRevenue     { get; set; }
        public int PendingLabOrders     { get; set; }
        public int PendingPrescriptions { get; set; }
        public int PendingBills         { get; set; }
        public List<HospitalEHR.Models.Patient> RecentPatients { get; set; } = new();
    }
}
