using System.ComponentModel.DataAnnotations;

namespace HospitalEHR.ViewModels
{
    public class EhrCreateVM
    {
        public int    PatientId   { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chief complaint is required")]
        [Display(Name = "Chief Complaint")]
        public string ChiefComplaint { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Symptoms { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Diagnosis { get; set; } = string.Empty;

        [StringLength(2000)] [Display(Name = "Clinical Notes")]
        public string? ClinicalNotes { get; set; }

        [StringLength(500)] [Display(Name = "Treatment Advised")]
        public string? TreatmentAdvised { get; set; }

        [DataType(DataType.Date)] [Display(Name = "Follow-Up Date")]
        public DateTime? FollowUpDate { get; set; }
    }

    public class LabOrderCreateVM
    {
        public int    PatientId   { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required, StringLength(150)] [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;

        [StringLength(500)] [Display(Name = "Instructions for Lab")]
        public string? DoctorInstructions { get; set; }
    }

    public class LabResultVM
    {
        public int    LabOrderId  { get; set; }
        public string TestName    { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Result { get; set; } = string.Empty;

        [StringLength(500)] [Display(Name = "Lab Notes")]
        public string? LabNotes { get; set; }

        [Required, Range(0, 999999)] [Display(Name = "Test Cost (Rs.)")]
        public decimal TestCost { get; set; }
    }

    public class PrescriptionCreateVM
    {
        public int    PatientId   { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required, StringLength(150)] [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Duration { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Instructions { get; set; }
    }

    public class DispenseVM
    {
        public int    PrescriptionId { get; set; }
        public string MedicineName   { get; set; } = string.Empty;
        public string PatientName    { get; set; } = string.Empty;

        [Required, Range(0, 999999)] [Display(Name = "Medicine Cost (Rs.)")]
        public decimal MedicineCost { get; set; }
    }

    public class TreatmentCreateVM
    {
        public int    PatientId   { get; set; }
        public string PatientName { get; set; } = string.Empty;

        [Required, StringLength(500)] [Display(Name = "Treatment Description")]
        public string TreatmentDescription { get; set; } = string.Empty;

        [StringLength(1000)] public string? Instructions { get; set; }
        [StringLength(200)]  public string? Medications  { get; set; }

        [DataType(DataType.Date)] [Display(Name = "Follow-Up Date")]
        public DateTime? FollowUpDate { get; set; }
    }
}
