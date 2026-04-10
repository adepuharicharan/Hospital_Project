using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Security;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _us;
        private readonly ILogger<AccountController> _log;

        public AccountController(IUserService us, ILogger<AccountController> log)
        { _us = us; _log = log; }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            string role = "Adminn";
            if (HttpContext.Session.IsAuthenticated()) return RedirectToDashboard();
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var user = await _us.GetByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Invalid credentials or account is deactivated.");
                return View(model);
            }

            if (!PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                _log.LogWarning("Failed login for {Email}", model.Email);
                return View(model);
            }

            HttpContext.Session.SetString(SessionKeys.UserId,   user.UserId.ToString());
            HttpContext.Session.SetString(SessionKeys.FullName, user.FullName);
            HttpContext.Session.SetString(SessionKeys.Email,    user.Email);
            HttpContext.Session.SetString(SessionKeys.Role,     user.Role.ToString());

            user.LastLoginAt = DateTime.UtcNow;
            await _us.UpdateAsync(user);
            _log.LogInformation("User {Email} ({Role}) logged in", user.Email, user.Role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard();
        }

        [HttpPost, ValidateAntiForgeryToken, SessionAuth]
        public IActionResult Logout()
        {
            _log.LogInformation("User {Email} logged out", HttpContext.Session.GetEmail());
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, SessionAuth]
        public IActionResult ChangePassword() => View(new ChangePasswordVM());

        [HttpPost, SessionAuth, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = HttpContext.Session.GetUserId();
            if (userId == null) return RedirectToAction(nameof(Login));

            var ok = await _us.ChangePasswordAsync(userId.Value, model.CurrentPassword, model.NewPassword);
            if (!ok) { ModelState.AddModelError("", "Current password is incorrect."); return View(model); }

            TempData["Success"] = "Password changed successfully.";
            return RedirectToDashboard();
        }

        public IActionResult AccessDenied() => View();

        private IActionResult RedirectToDashboard() =>
            HttpContext.Session.GetRole() switch
            {
                "Admin"          => RedirectToAction("Dashboard", "Admin"),
                "Receptionist"   => RedirectToAction("Dashboard", "Receptionist"),
                "Doctor"         => RedirectToAction("Dashboard", "Doctor"),
                "LabTechnician"  => RedirectToAction("Dashboard", "LabTechnician"),
                "Pharmacist"     => RedirectToAction("Dashboard", "Pharmacist"),
                "BillingOfficer" => RedirectToAction("Dashboard", "BillingOfficer"),
                _                => RedirectToAction("Login")
            };
    }
}
