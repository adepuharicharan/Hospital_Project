using HospitalEHR.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HospitalEHR.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!HttpContext.Session.IsAuthenticated())
                return RedirectToAction("Login", "Account");

            return HttpContext.Session.GetRole() switch
            {
                "Admin"          => RedirectToAction("Dashboard", "Admin"),
                "Receptionist"   => RedirectToAction("Dashboard", "Receptionist"),
                "Doctor"         => RedirectToAction("Dashboard", "Doctor"),
                "LabTechnician"  => RedirectToAction("Dashboard", "LabTechnician"),
                "Pharmacist"     => RedirectToAction("Dashboard", "Pharmacist"),
                "BillingOfficer" => RedirectToAction("Dashboard", "BillingOfficer"),
                _                => RedirectToAction("Login", "Account")
            };
        }

        public IActionResult Error() => View();
    }
}
