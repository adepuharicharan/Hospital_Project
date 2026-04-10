using HospitalEHR.Models;
using HospitalEHR.Security;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db, ILogger logger)
        {
            if (await db.AppUsers.AnyAsync()) return;

            var users = new[]
            {
                new AppUser { FullName="System Administrator",  Email="admin@hospital.com",        PasswordHash=PasswordHasher.Hash("Admin@123"),   Role=UserRole.Admin,          EmployeeId="EMP001", IsActive=true },
                new AppUser { FullName="Mary Receptionist",     Email="receptionist@hospital.com", PasswordHash=PasswordHasher.Hash("Recep@123"),   Role=UserRole.Receptionist,   EmployeeId="EMP002", IsActive=true },
                new AppUser { FullName="Dr. Arjun Sharma",      Email="doctor1@hospital.com",      PasswordHash=PasswordHasher.Hash("Doctor@123"),  Role=UserRole.Doctor,         EmployeeId="DOC001", Specialization="General Medicine", IsActive=true },
                new AppUser { FullName="Dr. Priya Nair",        Email="doctor2@hospital.com",      PasswordHash=PasswordHasher.Hash("Doctor@123"),  Role=UserRole.Doctor,         EmployeeId="DOC002", Specialization="Cardiology",      IsActive=true },
                new AppUser { FullName="Dr. Ravi Kumar",        Email="doctor3@hospital.com",      PasswordHash=PasswordHasher.Hash("Doctor@123"),  Role=UserRole.Doctor,         EmployeeId="DOC003", Specialization="Orthopedics",     IsActive=true },
                new AppUser { FullName="Lab Tech Suresh",       Email="lab1@hospital.com",         PasswordHash=PasswordHasher.Hash("Lab@12345"),   Role=UserRole.LabTechnician,  EmployeeId="LAB001", IsActive=true },
                new AppUser { FullName="Lab Tech Meena",        Email="lab2@hospital.com",         PasswordHash=PasswordHasher.Hash("Lab@12345"),   Role=UserRole.LabTechnician,  EmployeeId="LAB002", IsActive=true },
                new AppUser { FullName="Pharmacist Sara",       Email="pharmacist@hospital.com",   PasswordHash=PasswordHasher.Hash("Pharma@123"),  Role=UserRole.Pharmacist,     EmployeeId="PHR001", IsActive=true },
                new AppUser { FullName="Billing Officer Tom",   Email="billing@hospital.com",      PasswordHash=PasswordHasher.Hash("Billing@123"), Role=UserRole.BillingOfficer, EmployeeId="BIL001", IsActive=true },
            };

            await db.AppUsers.AddRangeAsync(users);
            await db.SaveChangesAsync();
            foreach (var u in users)
                logger.LogInformation("Seeded {Role}: {Email}", u.Role, u.Email);
        }
    }
}
