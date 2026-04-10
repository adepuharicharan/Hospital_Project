using AspNetCoreGeneratedDocument;
using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PatientService> _log;
		
		public PatientService(ApplicationDbContext db, ILogger<PatientService> log)
        { _db = db; _log = log;
		}

		public async Task<List<Patient>> GetAllAsync(string? search = null)
        {
            var q = _db.Patients.Include(p => p.AssignedDoctor).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id))
                    q = q.Where(p => p.PatientId == id);
                else
                    q = q.Where(p => p.FullName.ToLower().Contains(s)
                                  || p.ContactNumber.Contains(s)
                                  || (p.Email != null && p.Email.ToLower().Contains(s)));
            }
            return await q.OrderByDescending(p => p.RegistrationDate).ToListAsync();
        }

        public async Task<List<Patient>> GetByDoctorAsync(int doctorId, string? search = null)
        {
            var q = _db.Patients.Include(p => p.AssignedDoctor)
                       .Where(p => p.AssignedDoctorId == doctorId).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id)) q = q.Where(p => p.PatientId == id);
                else q = q.Where(p => p.FullName.ToLower().Contains(s));
            }
            return await q.OrderByDescending(p => p.RegistrationDate).ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id) =>
            await _db.Patients
                .Include(p => p.AssignedDoctor)
                .Include(p => p.EhrRecords).ThenInclude(e => e.Doctor)
                .Include(p => p.LabOrders).ThenInclude(l => l.OrderedByDoctor)
                .Include(p => p.TreatmentPlans).ThenInclude(t => t.Doctor)
                .Include(p => p.Prescriptions).ThenInclude(r => r.PrescribedByDoctor)
                .Include(p => p.BillingRecords)
                .FirstOrDefaultAsync(p => p.PatientId == id);

        public async Task<Patient> RegisterAsync(PatientRegisterVM vm)
        {
            var p = new Patient
            {
                FullName               = vm.FullName,
                DateOfBirth            = vm.DateOfBirth,
                Gender                 = vm.Gender,
                ContactNumber          = vm.ContactNumber,
                Email                  = vm.Email,
                Address                = vm.Address,
                City                   = vm.City,
                BloodGroup             = vm.BloodGroup,
                KnownAllergies         = vm.KnownAllergies,
                EmergencyContactName   = vm.EmergencyContactName,
                EmergencyContactNumber = vm.EmergencyContactNumber,
                AssignedDoctorId       = vm.AssignedDoctorId,
                RegistrationFee        = vm.RegistrationFee,
                ChiefComplaint         = vm.ChiefComplaint,
                AdmissionStatus        = AdmissionStatus.Registered,
                RegistrationDate       = DateTime.Now
            };
            _db.Patients.Add(p);
            await _db.SaveChangesAsync();
            _log.LogInformation("Patient registered: {Name} #{Id}", p.FullName, p.PatientId);
            return p;
        }

        public async Task<bool> UpdateAsync(Patient p)
        {
            try {
            _db.Patients.Update(p);
            await _db.SaveChangesAsync(); return true;
            }
            catch (Exception ex) { _log.LogError(ex, "UpdatePatient failed #{Id}", p.PatientId); return false; }
        }

        public async Task<bool> AdmitAsync(int id)
        {
            var p = await _db.Patients.FindAsync(id); if (p == null) return false;
            p.AdmissionStatus = AdmissionStatus.Admitted;
            p.AdmissionDate   = DateTime.Now;
            await _db.SaveChangesAsync(); return true;
        }

        public async Task<bool> DischargeAsync(int id)
        {
            var p = await _db.Patients.FindAsync(id); 
            if (p == null) return false;
            bool paid = await _db.BillingRecords
                .AnyAsync(b => b.PatientId == id && b.PaymentStatus == PaymentStatus.Paid);
            if (!paid) { _log.LogWarning("Discharge blocked — unpaid bill #{Id}", id); return false; }
            p.AdmissionStatus = AdmissionStatus.Discharged;
            p.DischargeDate   = DateTime.Now;
            await _db.SaveChangesAsync(); return true;
        }
    }
}
