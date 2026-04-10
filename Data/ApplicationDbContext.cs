using HospitalEHR.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AppUser>       AppUsers       { get; set; }
        public DbSet<Patient>       Patients       { get; set; }
        public DbSet<EhrRecord>     EhrRecords     { get; set; }
        public DbSet<LabOrder>      LabOrders      { get; set; }
        public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
        public DbSet<Prescription>  Prescriptions  { get; set; }
        public DbSet<BillingRecord> BillingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Enums stored as strings
            b.Entity<AppUser>()      .Property(u => u.Role)           .HasConversion<string>().HasMaxLength(30);
            b.Entity<Patient>()      .Property(p => p.AdmissionStatus).HasConversion<string>().HasMaxLength(30);
            b.Entity<Patient>()      .Property(p => p.Gender)         .HasConversion<string>().HasMaxLength(20);
            b.Entity<Patient>()      .Property(p => p.BloodGroup)     .HasConversion<string>().HasMaxLength(20);
            b.Entity<LabOrder>()     .Property(l => l.Status)         .HasConversion<string>().HasMaxLength(20);
            b.Entity<Prescription>() .Property(p => p.Status)         .HasConversion<string>().HasMaxLength(20);
            b.Entity<BillingRecord>().Property(r => r.PaymentStatus)  .HasConversion<string>().HasMaxLength(20);

            // Unique email
            b.Entity<AppUser>().HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_AppUser_Email");

            // Relationships
            b.Entity<Patient>().HasOne(p => p.AssignedDoctor).WithMany(u => u.AssignedPatients)
                .HasForeignKey(p => p.AssignedDoctorId).OnDelete(DeleteBehavior.SetNull);

            b.Entity<EhrRecord>().HasOne(e => e.Patient).WithMany(p => p.EhrRecords)
                .HasForeignKey(e => e.PatientId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<EhrRecord>().HasOne(e => e.Doctor).WithMany(u => u.EhrRecords)
                .HasForeignKey(e => e.DoctorId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<LabOrder>().HasOne(l => l.Patient).WithMany(p => p.LabOrders)
                .HasForeignKey(l => l.PatientId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<LabOrder>().HasOne(l => l.OrderedByDoctor).WithMany(u => u.LabOrders)
                .HasForeignKey(l => l.OrderedByDoctorId).OnDelete(DeleteBehavior.Restrict);
            b.Entity<LabOrder>().HasOne(l => l.CompletedByLabTech).WithMany()
                .HasForeignKey(l => l.CompletedByLabTechId).OnDelete(DeleteBehavior.SetNull);

            b.Entity<TreatmentPlan>().HasOne(t => t.Patient).WithMany(p => p.TreatmentPlans)
                .HasForeignKey(t => t.PatientId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<TreatmentPlan>().HasOne(t => t.Doctor).WithMany()
                .HasForeignKey(t => t.DoctorId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<Prescription>().HasOne(p => p.Patient).WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.PatientId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<Prescription>().HasOne(p => p.PrescribedByDoctor).WithMany()
                .HasForeignKey(p => p.PrescribedByDoctorId).OnDelete(DeleteBehavior.Restrict);
            b.Entity<Prescription>().HasOne(p => p.DispensedByPharmacist).WithMany()
                .HasForeignKey(p => p.DispensedByPharmacistId).OnDelete(DeleteBehavior.SetNull);

            b.Entity<BillingRecord>().HasOne(r => r.Patient).WithMany(p => p.BillingRecords)
                .HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<BillingRecord>().HasOne(r => r.GeneratedByOfficer).WithMany()
                .HasForeignKey(r => r.GeneratedByOfficerId).OnDelete(DeleteBehavior.SetNull);

            // Indexes
            b.Entity<Patient>()      .HasIndex(p => p.ContactNumber) .HasDatabaseName("IX_Patient_Contact");
            b.Entity<Patient>()      .HasIndex(p => p.AdmissionStatus).HasDatabaseName("IX_Patient_Status");
            b.Entity<LabOrder>()     .HasIndex(l => l.Status)         .HasDatabaseName("IX_Lab_Status");
            b.Entity<BillingRecord>().HasIndex(r => r.PaymentStatus)  .HasDatabaseName("IX_Bill_Status");
        }
    }
}
