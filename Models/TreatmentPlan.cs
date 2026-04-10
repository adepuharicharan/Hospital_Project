using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class TreatmentPlan
    {
        [Key] public int TreatmentId { get; set; }
        [Required] public int PatientId { get; set; }
        [Required] public int DoctorId  { get; set; }

        [Required, StringLength(500)] [Display(Name = "Treatment Description")]
        public string TreatmentDescription { get; set; } = string.Empty;

        [StringLength(1000)] public string? Instructions { get; set; }
        [StringLength(200)]  public string? Medications  { get; set; }

        public DateTime  TreatmentDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)] [Display(Name = "Follow-Up Date")]
        public DateTime? FollowUpDate { get; set; }

        [Column(TypeName = "decimal(10,2)"), Range(0, 999999)]
        [Display(Name = "Consultation Fee (₹)")]
        public decimal ConsultationFee { get; set; } = 0;

        [ForeignKey("PatientId")] public Patient? Patient { get; set; }
        [ForeignKey("DoctorId")]  public AppUser? Doctor  { get; set; }
    }
}
