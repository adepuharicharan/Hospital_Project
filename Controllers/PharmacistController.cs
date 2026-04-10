using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    [SessionAuth("Pharmacist","Admin")]
    public class PharmacistController : Controller
    {
        private readonly IPrescriptionService _rx;
        public PharmacistController(IPrescriptionService rx) => _rx = rx;

        public async Task<IActionResult> Dashboard(string? search)
        {
            var all = await _rx.GetAllAsync(search);
            ViewBag.Total     = all.Count;
            ViewBag.Pending   = all.Count(p => p.Status == PrescriptionStatus.Prescribed);
            ViewBag.Dispensed = all.Count(p => p.Status == PrescriptionStatus.Dispensed);
            ViewBag.Search    = search;
            return View(await _rx.GetPendingAsync());
        }

        public async Task<IActionResult> AllPrescriptions(string? search)
        {
            ViewBag.Search = search;
            return View(await _rx.GetAllAsync(search));
        }

        [HttpGet]
        public async Task<IActionResult> Dispense(int id)
        {
            var rx = await _rx.GetByIdAsync(id); if (rx == null) return NotFound();
            return View(new DispenseVM
            {
                PrescriptionId = rx.PrescriptionId,
                MedicineName   = rx.MedicineName,
                PatientName    = rx.Patient?.FullName ?? "N/A"
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Dispense(DispenseVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var pharmacistId = HttpContext.Session.GetUserId()!.Value;
            var ok = await _rx.DispenseAsync(model.PrescriptionId, pharmacistId, model.MedicineCost);
            TempData[ok ? "Success" : "Error"] = ok ? "Medicine dispensed successfully." : "Dispense failed.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
