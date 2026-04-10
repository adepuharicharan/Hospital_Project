using HospitalEHR.Models;
using HospitalEHR.ViewModels;

namespace HospitalEHR.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<List<TreatmentPlan>> GetByPatientAsync(int patientId);
        Task<TreatmentPlan> CreateAsync(TreatmentCreateVM vm, int doctorId);
    }
}
