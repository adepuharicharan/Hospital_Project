using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class EhrService : IEhrService
    {
        private readonly ApplicationDbContext _db;
        public EhrService(ApplicationDbContext db) => _db = db;

        public async Task<List<EhrRecord>> GetByPatientAsync(int patientId) =>
            await _db.EhrRecords.Include(e => e.Doctor)
                .Where(e => e.PatientId == patientId)
                .OrderByDescending(e => e.VisitDate).ToListAsync();

        public async Task<EhrRecord?> GetByIdAsync(int id) =>
            await _db.EhrRecords.Include(e => e.Doctor).Include(e => e.Patient)
                .FirstOrDefaultAsync(e => e.EhrId == id);

        public async Task<EhrRecord> CreateAsync(EhrCreateVM vm, int doctorId)
        {
            var r = new EhrRecord
            {
                PatientId        = vm.PatientId,
                DoctorId         = doctorId,
                ChiefComplaint   = vm.ChiefComplaint,
                Symptoms         = vm.Symptoms,
                Diagnosis        = vm.Diagnosis,
                ClinicalNotes    = vm.ClinicalNotes,
                TreatmentAdvised = vm.TreatmentAdvised,
                FollowUpDate     = vm.FollowUpDate,
                VisitDate        = DateTime.Now
            };
            _db.EhrRecords.Add(r);
            await _db.SaveChangesAsync();
            return r;
        }
    }
}
