using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class Patient
    {
        [Key] public int PatientId { get; set; }

        [Required, StringLength(100, MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z\s\.\-]+$", ErrorMessage = "Name cannot contain special characters or numbers")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [NotMapped] public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);

        [Required] public Gender Gender { get; set; }

        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Must be exactly 10 digits")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [EmailAddress, StringLength(150)]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)] public string? City { get; set; }

        [Display(Name = "Blood Group")]
        public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;

        [StringLength(200)] [Display(Name = "Known Allergies")]
        public string? KnownAllergies { get; set; }

        [StringLength(200)] [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Must be 10 digits")]
        [Display(Name = "Emergency Contact Number")]
        public string? EmergencyContactNumber { get; set; }

        public AdmissionStatus AdmissionStatus { get; set; } = AdmissionStatus.Registered;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime? AdmissionDate   { get; set; }
        public DateTime? DischargeDate   { get; set; }

        [StringLength(50)] [Display(Name = "Ward / Room")]
        public string? WardRoom { get; set; }

        public int? AssignedDoctorId { get; set; }

        [Column(TypeName = "decimal(10,2)"), Range(0, 999999)]
        [Display(Name = "Registration Fee (₹)")]
        public decimal RegistrationFee { get; set; } = 500;

        [StringLength(500)] [Display(Name = "Chief Complaint")]
        public string? ChiefComplaint { get; set; }

        // Navigation
        public AppUser?                   AssignedDoctor { get; set; }
        public ICollection<EhrRecord>     EhrRecords     { get; set; } = new List<EhrRecord>();
        public ICollection<LabOrder>      LabOrders      { get; set; } = new List<LabOrder>();
        public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
        public ICollection<BillingRecord> BillingRecords { get; set; } = new List<BillingRecord>();
        public ICollection<Prescription>  Prescriptions  { get; set; } = new List<Prescription>();
    }
}
