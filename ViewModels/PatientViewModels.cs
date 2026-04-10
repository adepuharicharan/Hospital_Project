using HospitalEHR.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalEHR.ViewModels
{
    public class PatientRegisterVM
    {
        [Required, StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\s\.\-]+$", ErrorMessage = "No special characters or numbers")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-30);

        [Required] public Gender Gender { get; set; }

        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Must be exactly 10 digits")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [EmailAddress, StringLength(150)]
        [Display(Name = "Email (optional)")]
        public string? Email { get; set; }

        [Required, StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)] public string? City { get; set; }

        [Display(Name = "Blood Group")]
        //public BloodGroup BloodGroup { get; set; }
		public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;

		[StringLength(200)] [Display(Name = "Known Allergies")]
        public string? KnownAllergies { get; set; }

        [StringLength(100)] [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Must be 10 digits")]
        [Display(Name = "Emergency Contact Number")]
        public string? EmergencyContactNumber { get; set; }

        [Required(ErrorMessage = "Please assign a doctor")]
        [Display(Name = "Assign Doctor")]
        public int AssignedDoctorId { get; set; }

        [Range(0, 999999)] [Display(Name = "Registration Fee (Rs.)")]
        public decimal RegistrationFee { get; set; } = 500;

        [StringLength(500)] [Display(Name = "Chief Complaint")]
        public string? ChiefComplaint { get; set; }

        public List<DoctorOptionVM> AvailableDoctors { get; set; } = new();
    }

    public class DoctorOptionVM
    {
        public int    UserId        { get; set; }
        public string FullName      { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Display => Specialization != null ? $"{FullName} — {Specialization}" : FullName;
    }
}
