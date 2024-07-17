using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.RESTServices.Interfaces;
using EPR.Payment.Portal.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    public class HomeController : Controller
    {

        private readonly DashboardConfiguration _dashboardConfiguration;

        public HomeController(IOptions<DashboardConfiguration> dashboardConfiguration)
        {
            _dashboardConfiguration = dashboardConfiguration.Value ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
        }

        public IActionResult Index()
        {
            // Use the static values from configuration
            //var menuUrl = _dashboardConfiguration.MenuUrl.Url ?? throw new InvalidOperationException("Menu Url is not configured.");
            //ViewBag.MenuUrl = menuUrl;
            var backUrl = _dashboardConfiguration.BackUrl.Url ?? throw new InvalidOperationException("Back Url is not configured.");
            ViewBag.BackUrl = backUrl;
            //var feedbackUrl = _dashboardConfiguration.FeedbackUrl.Url ?? throw new InvalidOperationException("Feedback Url is not configured.");
            //ViewBag.FeedbackUrl = feedbackUrl;
            var offlinePaymentUrl = _dashboardConfiguration.OfflinePaymentUrl.Url ?? throw new InvalidOperationException("Offline Payment Url is not configured.");
            ViewBag.OfflinePaymentUrl = offlinePaymentUrl;

            return View(_dashboardConfiguration);
        }
    }
}
