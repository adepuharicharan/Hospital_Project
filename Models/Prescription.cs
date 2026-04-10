using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class Prescription
    {
        [Key] public int PrescriptionId { get; set; }
        [Required] public int PatientId            { get; set; }
        [Required] public int PrescribedByDoctorId { get; set; }
        public int? DispensedByPharmacistId { get; set; }

        [Required, StringLength(150)] [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [Required, StringLength(100)] public string Dosage   { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Duration { get; set; } = string.Empty;

        [StringLength(500)] public string? Instructions { get; set; }

        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Prescribed;

        [Column(TypeName = "decimal(10,2)"), Range(0, 999999)]
        [Display(Name = "Medicine Cost (₹)")]
        public decimal MedicineCost { get; set; } = 0;

        public DateTime  PrescribedAt { get; set; } = DateTime.Now;
        public DateTime? DispensedAt  { get; set; }

        [ForeignKey("PatientId")]            
        public Patient? Patient                { get; set; }
        [ForeignKey("PrescribedByDoctorId")]    public AppUser? PrescribedByDoctor     { get; set; }
        [ForeignKey("DispensedByPharmacistId")] public AppUser? DispensedByPharmacist  { get; set; }
    }
}
