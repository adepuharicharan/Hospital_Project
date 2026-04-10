using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BillingService> _log;

        public BillingService(ApplicationDbContext db, ILogger<BillingService> log)
        { _db = db; _log = log; }

        public async Task<BillingRecord?> GetByPatientAsync(int patientId) =>
            await _db.BillingRecords.Include(b => b.Patient).Include(b => b.GeneratedByOfficer)
                .OrderByDescending(b => b.GeneratedAt)
                .FirstOrDefaultAsync(b => b.PatientId == patientId);

        public async Task<BillingRecord?> GetByIdAsync(int billId) =>
            await _db.BillingRecords
                .Include(b => b.Patient)
                .Include(b => b.GeneratedByOfficer)
                .FirstOrDefaultAsync(b => b.BillId == billId);

        public async Task<List<BillingRecord>> GetAllAsync(string? search = null)
        {
            var q = _db.BillingRecords.Include(b => b.Patient).Include(b => b.GeneratedByOfficer).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id)) q = q.Where(b => b.PatientId == id || b.BillId == id);
                else q = q.Where(b => b.Patient!.FullName.ToLower().Contains(s));
            }
            return await q.OrderByDescending(b => b.GeneratedAt).ToListAsync();
        }

        public async Task<BillingRecord> GenerateAsync(int patientId, int officerId)
        {
            var patient = await _db.Patients.FindAsync(patientId)
                ?? throw new InvalidOperationException($"Patient #{patientId} not found");

            var labFee  = await _db.LabOrders.Where(l => l.PatientId == patientId).SumAsync(l => l.TestCost);
            var medFee  = await _db.Prescriptions.Where(p => p.PatientId == patientId).SumAsync(p => p.MedicineCost);
            var consFee = await _db.TreatmentPlans.Where(t => t.PatientId == patientId).SumAsync(t => t.ConsultationFee);
            var total   = patient.RegistrationFee + labFee + medFee + consFee;

            var existing = await _db.BillingRecords
                .FirstOrDefaultAsync(b => b.PatientId == patientId && b.PaymentStatus == PaymentStatus.Pending);

            if (existing != null)
            {
                existing.RegistrationFee     = patient.RegistrationFee;
                existing.LabFee              = labFee;
                existing.MedicineFee         = medFee;
                existing.ConsultationFee     = consFee;
                existing.TotalAmount         = total;
                existing.GeneratedByOfficerId= officerId;
                existing.GeneratedAt         = DateTime.Now;
                await _db.SaveChangesAsync();
                return existing;
            }

            var bill = new BillingRecord
            {
                PatientId            = patientId,
                GeneratedByOfficerId = officerId,
                RegistrationFee      = patient.RegistrationFee,
                LabFee               = labFee,
                MedicineFee          = medFee,
                ConsultationFee      = consFee,
                TotalAmount          = total,
                PaymentStatus        = PaymentStatus.Pending,
                GeneratedAt          = DateTime.Now
            };
            _db.BillingRecords.Add(bill);
            await _db.SaveChangesAsync();
            _log.LogInformation("Bill #{BillId} generated for patient #{PatId} — Rs.{Total}", bill.BillId, patientId, total);
            return bill;
        }

        public async Task<bool> ProcessPaymentAsync(int billId, int officerId, string paymentMethod)
        {
            var b = await _db.BillingRecords.FindAsync(billId);
            if (b == null) return false;
            b.PaymentStatus        = PaymentStatus.Paid;
            b.PaidAt               = DateTime.Now;
            b.GeneratedByOfficerId = officerId;
            b.PaymentMethod        = paymentMethod;
            await _db.SaveChangesAsync();
            _log.LogInformation("Bill #{BillId} paid via {Method}", billId, paymentMethod);
            return true;
        }
    }
}
