using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    [SessionAuth("Receptionist","Admin")]
    public class ReceptionistController : Controller
    {
        private readonly IPatientService _ps;
        private readonly IUserService    _us;
        private readonly ILogger<ReceptionistController> _log;

        public ReceptionistController(IPatientService ps, IUserService us, ILogger<ReceptionistController> log)
        { _ps = ps; _us = us; _log = log; }

        public async Task<IActionResult> Dashboard(string? search)
        {
            var patients = await _ps.GetAllAsync(search);
            ViewBag.Registered = patients.Count(p => p.AdmissionStatus == AdmissionStatus.Registered);
            ViewBag.Admitted   = patients.Count(p => p.AdmissionStatus == AdmissionStatus.Admitted);
            ViewBag.Discharged = patients.Count(p => p.AdmissionStatus == AdmissionStatus.Discharged);
            ViewBag.Search     = search;
            return View(patients.Take(10).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> RegisterPatient()
        {
            var doctors = await _us.GetByRoleAsync(UserRole.Doctor);
            return View(new PatientRegisterVM
            {
                AvailableDoctors = doctors.Select(d => new DoctorOptionVM
                    { UserId = d.UserId, FullName = d.FullName, Specialization = d.Specialization }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient(PatientRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                var doctors = await _us.GetByRoleAsync(UserRole.Doctor);
                model.AvailableDoctors = doctors.Select(d => new DoctorOptionVM
                    { UserId = d.UserId, FullName = d.FullName, Specialization = d.Specialization }).ToList();
                return View(model);
            }
            var p = await _ps.RegisterAsync(model);
            TempData["Success"] = $"Patient \'{p.FullName}\' registered! ID: #{p.PatientId}";
            return RedirectToAction(nameof(PatientList));
        }

        public async Task<IActionResult> PatientList(string? search)
        {
            ViewBag.Search = search;
            return View(await _ps.GetAllAsync(search));
        }

        public async Task<IActionResult> PatientDetails(int id)
        {
            var p = await _ps.GetByIdAsync(id);
            if (p == null) { TempData["Error"] = "Patient not found."; return RedirectToAction(nameof(PatientList)); }
            return View(p);
        }

        [HttpGet]
        public async Task<IActionResult> EditPatient(int id)
        {
            var p = await _ps.GetByIdAsync(id); if (p == null) return NotFound();
            ViewBag.Doctors = await _us.GetByRoleAsync(UserRole.Doctor);
            return View(p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(Patient model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctors = await _us.GetByRoleAsync(UserRole.Doctor);
                return View(model);
            }
            await _ps.UpdateAsync(model);
            TempData["Success"] = "Patient information updated.";
            return RedirectToAction(nameof(PatientDetails), new { id = model.PatientId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AdmitPatient(int patientId)
        {
            var ok = await _ps.AdmitAsync(patientId);
            TempData[ok ? "Success" : "Error"] = ok ? "Patient admitted to ward." : "Admission failed.";
            return RedirectToAction(nameof(PatientDetails), new { id = patientId });
        }
    }
}
