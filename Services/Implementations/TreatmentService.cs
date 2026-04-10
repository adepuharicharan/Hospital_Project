using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class TreatmentService : ITreatmentService
    {
        private readonly ApplicationDbContext _db;
        public TreatmentService(ApplicationDbContext db) => _db = db;

        public async Task<List<TreatmentPlan>> GetByPatientAsync(int patientId) =>
            await _db.TreatmentPlans.Include(t => t.Doctor)
                .Where(t => t.PatientId == patientId)
                .OrderByDescending(t => t.TreatmentDate).ToListAsync();

        public async Task<TreatmentPlan> CreateAsync(TreatmentCreateVM vm, int doctorId)
        {
            var t = new TreatmentPlan
            {
                PatientId            = vm.PatientId,
                DoctorId             = doctorId,
                TreatmentDescription = vm.TreatmentDescription,
                Instructions         = vm.Instructions,
                Medications          = vm.Medications,
                FollowUpDate         = vm.FollowUpDate,
                ConsultationFee      = 0,
                TreatmentDate        = DateTime.Now
            };
            _db.TreatmentPlans.Add(t);
            await _db.SaveChangesAsync();
            return t;
        }
    }
}
