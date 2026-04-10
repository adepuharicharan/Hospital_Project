using HospitalEHR.Models;
using HospitalEHR.ViewModels;

namespace HospitalEHR.Services.Interfaces
{
    public interface ILabOrderService
    {
        Task<List<LabOrder>> GetAllAsync(string? search = null);
        Task<List<LabOrder>> GetByPatientAsync(int patientId);
        Task<LabOrder?> GetByIdAsync(int id);
        Task<LabOrder> CreateAsync(LabOrderCreateVM vm, int doctorId);
        Task<bool> UpdateResultAsync(LabResultVM vm, int techId);
    }
}
