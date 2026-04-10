using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class BillingRecord
    {
        [Key] public int BillId { get; set; }
        [Required] public int PatientId { get; set; }
        public int? GeneratedByOfficerId { get; set; }

        [Column(TypeName = "decimal(10,2)")] public decimal RegistrationFee { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal ConsultationFee { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal LabFee          { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal MedicineFee     { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal OtherCharges    { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal Discount        { get; set; } = 0;
        [Column(TypeName = "decimal(10,2)")] public decimal TotalAmount     { get; set; } = 0;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [StringLength(100)] [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(500)] public string? Remarks { get; set; }

        public DateTime  GeneratedAt { get; set; } = DateTime.Now;
        public DateTime? PaidAt      { get; set; }

        [ForeignKey("PatientId")]            
        public Patient? Patient             { get; set; }
        [ForeignKey("GeneratedByOfficerId")] public AppUser? GeneratedByOfficer  { get; set; }
    }
}
