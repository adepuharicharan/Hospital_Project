using HospitalEHR.Data;
using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Controllers
{
    [SessionAuth("Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _us;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminController> _log;

        public AdminController(IUserService us, ApplicationDbContext db, ILogger<AdminController> log)
        { _us = us; _db = db; _log = log; }

        public async Task<IActionResult> Dashboard()
        {
            var vm = new AdminDashVM
            {
                TotalPatients        = await _db.Patients.CountAsync(),
                AdmittedCount        = await _db.Patients.CountAsync(p => p.AdmissionStatus == AdmissionStatus.Admitted),
                TotalDoctors         = await _db.AppUsers.CountAsync(u => u.Role == UserRole.Doctor && u.IsActive),
                TotalUsers           = await _db.AppUsers.CountAsync(),
                TotalRevenue         = await _db.BillingRecords.Where(b => b.PaymentStatus == PaymentStatus.Paid).SumAsync(b => b.TotalAmount),
                PendingLabOrders     = await _db.LabOrders.CountAsync(l => l.Status == TestStatus.Ordered),
                PendingPrescriptions = await _db.Prescriptions.CountAsync(p => p.Status == PrescriptionStatus.Prescribed),
                PendingBills         = await _db.BillingRecords.CountAsync(b => b.PaymentStatus == PaymentStatus.Pending),
                RecentPatients       = await _db.Patients.Include(p => p.AssignedDoctor)
                                           .OrderByDescending(p => p.RegistrationDate).Take(10).ToListAsync()
            };
            return View(vm);
        }

        public async Task<IActionResult> Users(string? search, string? roleFilter)
        {
            var users = await _us.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(search))
                users = users.Where(u => u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(roleFilter) && Enum.TryParse<UserRole>(roleFilter, out var role))
                users = users.Where(u => u.Role == role).ToList();

            ViewBag.Search     = search;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.AllRoles   = Enum.GetNames<UserRole>();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = Enum.GetValues<UserRole>();
            return View(new CreateUserVM());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            ViewBag.Roles = Enum.GetValues<UserRole>();
            if (!ModelState.IsValid) return View(model);

            if (await _db.AppUsers.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new AppUser
            {
                FullName       = model.FullName, Email = model.Email,
                Role           = model.Role,     Specialization = model.Specialization,
                EmployeeId     = model.EmployeeId, IsActive = true
            };

            if (!await _us.CreateAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Failed to create user. Please try again.");
                return View(model);
            }

            TempData["Success"] = $"User '{model.FullName}' created as {model.Role}.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            if (userId == HttpContext.Session.GetUserId())
            {
                TempData["Error"] = "You cannot deactivate your own account.";
                return RedirectToAction(nameof(Users));
            }
            var u = await _us.GetByIdAsync(userId);
            if (u == null) return NotFound();
            await _us.ToggleStatusAsync(userId);
            TempData["Success"] = $"User '{u.FullName}' has been {(u.IsActive ? "deactivated" : "activated")}.";
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> AllPatients(string? search, string? status)
        {
            var q = _db.Patients.Include(p => p.AssignedDoctor).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                if (int.TryParse(s, out int id)) q = q.Where(p => p.PatientId == id);
                else q = q.Where(p => p.FullName.ToLower().Contains(s) || p.ContactNumber.Contains(s));
            }
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AdmissionStatus>(status, out var st))
                q = q.Where(p => p.AdmissionStatus == st);

            ViewBag.Search = search; ViewBag.Status = status;
            return View(await q.OrderByDescending(p => p.RegistrationDate).ToListAsync());
        }

        public async Task<IActionResult> Reports()
        {
            ViewBag.TotalPatients  = await _db.Patients.CountAsync();
            ViewBag.Registered     = await _db.Patients.CountAsync(p => p.AdmissionStatus == AdmissionStatus.Registered);
            ViewBag.Admitted       = await _db.Patients.CountAsync(p => p.AdmissionStatus == AdmissionStatus.Admitted);
            ViewBag.Discharged     = await _db.Patients.CountAsync(p => p.AdmissionStatus == AdmissionStatus.Discharged);
            ViewBag.Revenue        = await _db.BillingRecords.Where(b => b.PaymentStatus == PaymentStatus.Paid).SumAsync(b => b.TotalAmount);
            ViewBag.PendingRevenue = await _db.BillingRecords.Where(b => b.PaymentStatus == PaymentStatus.Pending).SumAsync(b => b.TotalAmount);
            ViewBag.LabOrders      = await _db.LabOrders.CountAsync();
            ViewBag.LabCompleted   = await _db.LabOrders.CountAsync(l => l.Status == TestStatus.Completed);
            ViewBag.Prescriptions  = await _db.Prescriptions.CountAsync();
            ViewBag.PrescDispensed = await _db.Prescriptions.CountAsync(p => p.Status == PrescriptionStatus.Dispensed);
            return View();
        }
    }
}
