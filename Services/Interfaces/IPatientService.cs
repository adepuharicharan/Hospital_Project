using HospitalEHR.Models;
using HospitalEHR.ViewModels;

namespace HospitalEHR.Services.Interfaces
{
    public interface IPatientService
    {
        Task<List<Patient>> GetAllAsync(string? search = null);
        Task<List<Patient>> GetByDoctorAsync(int doctorId, string? search = null);
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> RegisterAsync(PatientRegisterVM vm);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> AdmitAsync(int id);
        Task<bool> DischargeAsync(int id);
    }
}
