using HospitalEHR.Models;
using HospitalEHR.ViewModels;

namespace HospitalEHR.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<List<Prescription>> GetAllAsync(string? search = null);
        Task<List<Prescription>> GetByPatientAsync(int patientId);
        Task<List<Prescription>> GetPendingAsync();
        Task<Prescription?> GetByIdAsync(int id);
        Task<Prescription> CreateAsync(PrescriptionCreateVM vm, int doctorId);
        Task<bool> DispenseAsync(int id, int pharmacistId, decimal cost);
    }
}
