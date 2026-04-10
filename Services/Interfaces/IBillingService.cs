using HospitalEHR.Models;

namespace HospitalEHR.Services.Interfaces
{
    public interface IBillingService
    {
        Task<BillingRecord?> GetByPatientAsync(int patientId);
        Task<BillingRecord?> GetByIdAsync(int billId);
        Task<List<BillingRecord>> GetAllAsync(string? search = null);
        Task<BillingRecord> GenerateAsync(int patientId, int officerId);
        Task<bool> ProcessPaymentAsync(int billId, int officerId, string paymentMethod);
    }
}
