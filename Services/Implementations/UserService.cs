using HospitalEHR.Data;
using HospitalEHR.Models;
using HospitalEHR.Security;
using HospitalEHR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserService> _log;

        public UserService(ApplicationDbContext db, ILogger<UserService> log)
        { _db = db; _log = log; }

        public async Task<List<AppUser>> GetByRoleAsync(UserRole role) =>
            await _db.AppUsers.Where(u => u.Role == role && u.IsActive)
                              .OrderBy(u => u.FullName).ToListAsync();

        public async Task<List<AppUser>> GetAllAsync() =>
            await _db.AppUsers.OrderBy(u => u.FullName).ToListAsync();

        public async Task<AppUser?> GetByIdAsync(int id) =>
            await _db.AppUsers.FindAsync(id);

        public async Task<AppUser?> GetByEmailAsync(string email) =>
            await _db.AppUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

        public async Task<bool> CreateAsync(AppUser user, string password)
        {
            try
            {
                if (await _db.AppUsers.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower()))
                    return false;
                user.PasswordHash = PasswordHasher.Hash(password);
                user.CreatedAt    = DateTime.Now;
                _db.AppUsers.Add(user);
                await _db.SaveChangesAsync();
                _log.LogInformation("User created: {Email} as {Role}", user.Email, user.Role);
                return true;
            }
            catch (Exception ex) { _log.LogError(ex, "CreateUser failed {Email}", user.Email); return false; }
        }

        public async Task<bool> UpdateAsync(AppUser user)
        {
            try { _db.AppUsers.Update(user); await _db.SaveChangesAsync(); return true; }
            catch (Exception ex) { _log.LogError(ex, "UpdateUser failed #{Id}", user.UserId); return false; }
        }

        public async Task<bool> ToggleStatusAsync(int userId)
        {
            var u = await _db.AppUsers.FindAsync(userId);
            if (u == null) return false;
            u.IsActive = !u.IsActive;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var u = await _db.AppUsers.FindAsync(userId);
            if (u == null) return false;
            if (!PasswordHasher.Verify(currentPassword, u.PasswordHash)) return false;
            u.PasswordHash = PasswordHasher.Hash(newPassword);
            await _db.SaveChangesAsync();
            _log.LogInformation("Password changed for user #{Id}", userId);
            return true;
        }
    }
}
