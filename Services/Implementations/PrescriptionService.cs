using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ApplicationDbContext _db;
        public PrescriptionService(ApplicationDbContext db) => _db = db;

        public async Task<List<Prescription>> GetAllAsync(string? search = null)
        {
            var q = _db.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.PrescribedByDoctor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id))
                    q = q.Where(p => p.PatientId == id || p.PrescriptionId == id);
                else
                    q = q.Where(p => p.MedicineName.ToLower().Contains(s)
                                  || p.Patient!.FullName.ToLower().Contains(s));
            }
            return await q.OrderByDescending(p => p.PrescribedAt).ToListAsync();
        }

        public async Task<List<Prescription>> GetByPatientAsync(int patientId) =>
            await _db.Prescriptions.Include(p => p.PrescribedByDoctor)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PrescribedAt).ToListAsync();

        public async Task<List<Prescription>> GetPendingAsync() =>
            await _db.Prescriptions.Include(p => p.Patient).Include(p => p.PrescribedByDoctor)
                .Where(p => p.Status == PrescriptionStatus.Prescribed)
                .OrderByDescending(p => p.PrescribedAt).ToListAsync();

        public async Task<Prescription?> GetByIdAsync(int id) =>
            await _db.Prescriptions.Include(p => p.Patient).Include(p => p.PrescribedByDoctor)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

        public async Task<Prescription> CreateAsync(PrescriptionCreateVM vm, int doctorId)
        {
            var rx = new Prescription
            {
                PatientId            = vm.PatientId,
                PrescribedByDoctorId = doctorId,
                MedicineName         = vm.MedicineName,
                Dosage               = vm.Dosage,
                Duration             = vm.Duration,
                Instructions         = vm.Instructions,
                Status               = PrescriptionStatus.Prescribed,
                PrescribedAt         = DateTime.Now
            };
            _db.Prescriptions.Add(rx);
            await _db.SaveChangesAsync();
            return rx;
        }

        public async Task<bool> DispenseAsync(int id, int pharmacistId, decimal cost)
        {
            var rx = await _db.Prescriptions.FindAsync(id);
            if (rx == null) return false;
            rx.Status                  = PrescriptionStatus.Dispensed;
            rx.DispensedByPharmacistId = pharmacistId;
            rx.MedicineCost            = cost;
            rx.DispensedAt             = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
