using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    public class PaymentSuccessController : Controller
    {
        private readonly DashboardConfiguration _dashboardConfiguration;

        public PaymentSuccessController(IOptions<DashboardConfiguration> dashboardConfiguration)
        {
            _dashboardConfiguration = dashboardConfiguration.Value ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
        }

        public IActionResult Index(CompletePaymentViewModel completePaymentResponseViewModel)
        {
            ViewData["reference"] = completePaymentResponseViewModel.Reference;
            return View(_dashboardConfiguration);
        }
    }
}
