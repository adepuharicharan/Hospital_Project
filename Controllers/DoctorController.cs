using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    [SessionAuth("Doctor","Admin")]
    public class DoctorController : Controller
    {
        private readonly IPatientService      _ps;
        private readonly IEhrService          _ehr;
        private readonly ILabOrderService     _lab;
        private readonly IPrescriptionService _rx;
        private readonly ITreatmentService    _tx;
        private readonly ILogger<DoctorController> _log;

        public DoctorController(IPatientService ps, IEhrService ehr, ILabOrderService lab,
            IPrescriptionService rx, ITreatmentService tx, ILogger<DoctorController> log)
        { _ps = ps; _ehr = ehr; _lab = lab; _rx = rx; _tx = tx; _log = log; }

        private int  DoctorId => HttpContext.Session.GetUserId()!.Value;
        private bool IsAdmin  => HttpContext.Session.IsInRole("Admin");

        public async Task<IActionResult> Dashboard(string? search)
        {
            var patients = await _ps.GetByDoctorAsync(DoctorId, search);
            ViewBag.TotalPatients = patients.Count;
            ViewBag.Admitted      = patients.Count(p => p.AdmissionStatus == AdmissionStatus.Admitted);
            ViewBag.Registered    = patients.Count(p => p.AdmissionStatus == AdmissionStatus.Registered);
            ViewBag.Search = search;
            return View(patients);
        }

        public async Task<IActionResult> MyPatients(string? search)
        {
            ViewBag.Search = search;
            return View(await _ps.GetByDoctorAsync(DoctorId, search));
        }

        public async Task<IActionResult> PatientDetails(int id)
        {
            var p = await _ps.GetByIdAsync(id); if (p == null) return NotFound();
            if (p.AssignedDoctorId != DoctorId && !IsAdmin) return Forbid();
            ViewBag.EhrRecords    = await _ehr.GetByPatientAsync(id);
            ViewBag.LabOrders     = await _lab.GetByPatientAsync(id);
            ViewBag.Prescriptions = await _rx.GetByPatientAsync(id);
            ViewBag.Treatments    = await _tx.GetByPatientAsync(id);
            return View(p);
        }

        [HttpGet]
        public async Task<IActionResult> CreateEhr(int patientId)
        {
            var p = await _ps.GetByIdAsync(patientId); if (p == null) return NotFound();
            return View(new EhrCreateVM { PatientId = patientId, PatientName = p.FullName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEhr(EhrCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            await _ehr.CreateAsync(model, DoctorId);
            TempData["Success"] = "EHR record added successfully.";
            return RedirectToAction(nameof(PatientDetails), new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> AddTreatment(int patientId)
        {
            var p = await _ps.GetByIdAsync(patientId); if (p == null) return NotFound();
            return View(new TreatmentCreateVM { PatientId = patientId, PatientName = p.FullName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTreatment(TreatmentCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            await _tx.CreateAsync(model, DoctorId);
            TempData["Success"] = "Treatment plan added.";
            return RedirectToAction(nameof(PatientDetails), new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> OrderLabTest(int patientId)
        {
            var p = await _ps.GetByIdAsync(patientId); if (p == null) return NotFound();
            return View(new LabOrderCreateVM { PatientId = patientId, PatientName = p.FullName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderLabTest(LabOrderCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            await _lab.CreateAsync(model, DoctorId);
            TempData["Success"] = "Lab test ordered.";
            return RedirectToAction(nameof(PatientDetails), new { id = model.PatientId });
        }

        [HttpGet]
        public async Task<IActionResult> PrescribeMedicine(int patientId)
        {
            var p = await _ps.GetByIdAsync(patientId); if (p == null) return NotFound();
            return View(new PrescriptionCreateVM { PatientId = patientId, PatientName = p.FullName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> PrescribeMedicine(PrescriptionCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            await _rx.CreateAsync(model, DoctorId);
            TempData["Success"] = "Prescription added.";
            return RedirectToAction(nameof(PatientDetails), new { id = model.PatientId });
        }
    }
}
