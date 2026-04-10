using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class EhrRecord
    {
        [Key] public int EhrId { get; set; }
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId  { get; set; }

        [Required, StringLength(255)] [Display(Name = "Chief Complaint")]
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

        public DateTime VisitDate { get; set; } = DateTime.Now;

        [ForeignKey("PatientId")] public Patient?  Patient { get; set; }
        [ForeignKey("DoctorId")]  public AppUser?  Doctor  { get; set; }
    }
}
