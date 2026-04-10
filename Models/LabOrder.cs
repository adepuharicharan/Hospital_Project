using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalEHR.Models
{
    public class LabOrder
    {
        [Key] public int LabOrderId { get; set; }
        [Required] public int PatientId          { get; set; }
        [Required] public int OrderedByDoctorId  { get; set; }
        public int? CompletedByLabTechId { get; set; }

        [Required, StringLength(150)] [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;

        [StringLength(500)] [Display(Name = "Doctor's Instructions")]
        public string? DoctorInstructions { get; set; }

        public TestStatus Status { get; set; } = TestStatus.Ordered;

        [StringLength(1000)] [Display(Name = "Result")]
        public string? Result { get; set; }

        [StringLength(500)] [Display(Name = "Lab Notes")]
        public string? LabNotes { get; set; }

        [Column(TypeName = "decimal(10,2)"), Range(0, 999999)]
        [Display(Name = "Test Cost (₹)")]
        public decimal TestCost { get; set; } = 0;

        public DateTime  OrderedAt   { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("PatientId")]            public Patient?  Patient           { get; set; }
        [ForeignKey("OrderedByDoctorId")]    public AppUser?  OrderedByDoctor   { get; set; }
        [ForeignKey("CompletedByLabTechId")] public AppUser?  CompletedByLabTech{ get; set; }
    }
}
