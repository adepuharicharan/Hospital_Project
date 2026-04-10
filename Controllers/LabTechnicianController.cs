using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using HospitalEHR.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    [SessionAuth("LabTechnician","Admin")]
    public class LabTechnicianController : Controller
    {
        private readonly ILabOrderService _lab;
        public LabTechnicianController(ILabOrderService lab) => _lab = lab;

        public async Task<IActionResult> Dashboard(string? search)
        {
            var all = await _lab.GetAllAsync(search);
            ViewBag.TotalOrders = all.Count;
            ViewBag.Pending     = all.Count(o => o.Status == TestStatus.Ordered);
            ViewBag.InProgress  = all.Count(o => o.Status == TestStatus.InProgress);
            ViewBag.Completed   = all.Count(o => o.Status == TestStatus.Completed);
            ViewBag.Search      = search;
            return View(all.Where(o => o.Status != TestStatus.Completed).ToList());
        }

        public async Task<IActionResult> AllOrders(string? search)
        {
            ViewBag.Search = search;
            return View(await _lab.GetAllAsync(search));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateResult(int id)
        {
            var o = await _lab.GetByIdAsync(id); if (o == null) return NotFound();
            return View(new LabResultVM
            {
                LabOrderId  = o.LabOrderId,
                TestName    = o.TestName,
                PatientName = o.Patient?.FullName ?? "N/A",
                TestCost    = 0
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResult(LabResultVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var techId = HttpContext.Session.GetUserId()!.Value;
            var ok = await _lab.UpdateResultAsync(model, techId);
            TempData[ok ? "Success" : "Error"] = ok ? "Result submitted successfully." : "Update failed.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
