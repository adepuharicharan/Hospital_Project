using HospitalEHR.Models;

namespace HospitalEHR.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<AppUser>> GetByRoleAsync(UserRole role);
        Task<List<AppUser>> GetAllAsync();
        Task<AppUser?> GetByIdAsync(int id);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> CreateAsync(AppUser user, string password);
        Task<bool> UpdateAsync(AppUser user);
        Task<bool> ToggleStatusAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
