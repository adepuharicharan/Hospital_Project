using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class LabOrderService : ILabOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LabOrderService> _log;

        public LabOrderService(ApplicationDbContext db, ILogger<LabOrderService> log)
        { _db = db; _log = log; }

        public async Task<List<LabOrder>> GetAllAsync(string? search = null)
        {
            var q = _db.LabOrders
                .Include(l => l.Patient)
                .Include(l => l.OrderedByDoctor)
                .Include(l => l.CompletedByLabTech)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id))
                    q = q.Where(l => l.PatientId == id || l.LabOrderId == id);
                else
                    q = q.Where(l => l.TestName.ToLower().Contains(s)
                                  || l.Patient!.FullName.ToLower().Contains(s));
            }
            return await q.OrderByDescending(l => l.OrderedAt).ToListAsync();
        }

        public async Task<List<LabOrder>> GetByPatientAsync(int patientId) =>
            await _db.LabOrders.Include(l => l.OrderedByDoctor)
                .Where(l => l.PatientId == patientId)
                .OrderByDescending(l => l.OrderedAt).ToListAsync();

        public async Task<LabOrder?> GetByIdAsync(int id) =>
            await _db.LabOrders.Include(l => l.Patient).Include(l => l.OrderedByDoctor)
                .FirstOrDefaultAsync(l => l.LabOrderId == id);

        public async Task<LabOrder> CreateAsync(LabOrderCreateVM vm, int doctorId)
        {
            var o = new LabOrder
            {
                PatientId          = vm.PatientId,
                OrderedByDoctorId  = doctorId,
                TestName           = vm.TestName,
                DoctorInstructions = vm.DoctorInstructions,
                TestCost           = 0,
                Status             = TestStatus.Ordered,
                OrderedAt          = DateTime.Now
            };
            _db.LabOrders.Add(o);
            await _db.SaveChangesAsync();
            _log.LogInformation("Lab order '{Test}' for patient #{Id}", o.TestName, o.PatientId);
            return o;
        }

        public async Task<bool> UpdateResultAsync(LabResultVM vm, int techId)
        {
            var o = await _db.LabOrders.FindAsync(vm.LabOrderId);
            if (o == null) return false;
            o.Result               = vm.Result;
            o.LabNotes             = vm.LabNotes;
            o.TestCost             = vm.TestCost;
            o.Status               = TestStatus.Completed;
            o.CompletedByLabTechId = techId;
            o.CompletedAt          = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
