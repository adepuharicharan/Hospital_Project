using HospitalEHR.Models;
using HospitalEHR.ViewModels;

namespace HospitalEHR.Services.Interfaces
{
    public interface IEhrService
    {
        Task<List<EhrRecord>> GetByPatientAsync(int patientId);
        Task<EhrRecord?> GetByIdAsync(int id);
        Task<EhrRecord> CreateAsync(EhrCreateVM vm, int doctorId);
    }
}
