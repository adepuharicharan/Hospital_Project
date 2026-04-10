using HospitalEHR.Data;
using HospitalEHR.Extensions;
using HospitalEHR.Filters;
using HospitalEHR.Models;
using HospitalEHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalEHR.Controllers
{
    [SessionAuth("BillingOfficer","Admin")]
    public class BillingOfficerController : Controller
    {
        private readonly IBillingService  _bill;
        private readonly IPatientService  _ps;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BillingOfficerController> _log;

        public BillingOfficerController(IBillingService bill, IPatientService ps,
            ApplicationDbContext db, ILogger<BillingOfficerController> log)
        { _bill = bill; _ps = ps; _db = db; _log = log; }

        public async Task<IActionResult> Dashboard(string? search)
        {
            var bills = await _bill.GetAllAsync(search);
            ViewBag.TotalBills      = bills.Count;
            ViewBag.PendingPayments = bills.Count(b => b.PaymentStatus == PaymentStatus.Pending);
            ViewBag.PaidBills       = bills.Count(b => b.PaymentStatus == PaymentStatus.Paid);
            ViewBag.TotalRevenue    = bills.Where(b => b.PaymentStatus == PaymentStatus.Paid).Sum(b => b.TotalAmount);
            ViewBag.Search          = search;
            return View(bills.Take(10).ToList());
        }

        public async Task<IActionResult> PatientList(string? search)
        {
            var patients = await _ps.GetAllAsync(search);
            ViewBag.Search = search;
            return View(patients.Where(p => p.AdmissionStatus != AdmissionStatus.Discharged).ToList());
        }

        public async Task<IActionResult> GenerateBill(int patientId)
        {
            var officerId = HttpContext.Session.GetUserId()!.Value;
            var bill    = await _bill.GenerateAsync(patientId, officerId);
            var patient = await _ps.GetByIdAsync(patientId);
            ViewBag.Bill          = bill;
            ViewBag.LabOrders     = await _db.LabOrders.Include(l => l.OrderedByDoctor).Where(l => l.PatientId == patientId).ToListAsync();
            ViewBag.Prescriptions = await _db.Prescriptions.Include(p => p.PrescribedByDoctor).Where(p => p.PatientId == patientId).ToListAsync();
            ViewBag.Treatments    = await _db.TreatmentPlans.Include(t => t.Doctor).Where(t => t.PatientId == patientId).ToListAsync();
            return View(patient);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int billId, string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                TempData["Error"] = "Please select a payment method.";
                return RedirectToAction(nameof(Dashboard));
            }
            var officerId = HttpContext.Session.GetUserId()!.Value;
            var ok = await _bill.ProcessPaymentAsync(billId, officerId, paymentMethod);
            TempData[ok ? "Success" : "Error"] = ok
                ? $"Payment processed via {paymentMethod}."
                : "Payment failed.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DischargePatient(int patientId)
        {
            var ok = await _ps.DischargeAsync(patientId);
            TempData[ok ? "Success" : "Error"] = ok
                ? "Patient discharged successfully."
                : "Cannot discharge — payment must be PAID first.";
            return RedirectToAction(nameof(PatientList));
        }

        public async Task<IActionResult> AllBills(string? search)
        {
            ViewBag.Search = search;
            return View(await _bill.GetAllAsync(search));
        }

        // ── Invoice Print View ────────────────────────────────────────────────
        public async Task<IActionResult> Invoice(int billId)
        {
            var bill = await _bill.GetByIdAsync(billId);
            if (bill == null) return NotFound();
            var labs = await _db.LabOrders.Include(l => l.OrderedByDoctor).Where(l => l.PatientId == bill.PatientId).ToListAsync();
            var rxs  = await _db.Prescriptions.Include(p => p.PrescribedByDoctor).Where(p => p.PatientId == bill.PatientId).ToListAsync();
            var txs  = await _db.TreatmentPlans.Include(t => t.Doctor).Where(t => t.PatientId == bill.PatientId).ToListAsync();
            ViewBag.LabOrders     = labs;
            ViewBag.Prescriptions = rxs;
            ViewBag.Treatments    = txs;
            return View(bill);
        }
    }
}
