using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    [Route("GovPaySuccess", Name = RouteNames.GovPay.PaymentSuccess)]
    public class GovPaySuccessController : Controller
    {
        private readonly DashboardConfiguration _dashboardConfiguration;

        public GovPaySuccessController(IOptions<DashboardConfiguration> dashboardConfiguration)
        {
            _dashboardConfiguration = dashboardConfiguration?.Value
                ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
        }

        [HttpGet]
        public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
        {
            if (!ModelState.IsValid || completePaymentResponseViewModel is null)
            {
                return RedirectToRoute(RouteNames.GovPay.PaymentError, new { message = ExceptionMessages.ErrorInvalidViewModel });
            }

            var compositeViewModel = new CompositeViewModel
            {
                completePaymentViewModel = completePaymentResponseViewModel,
                dashboardConfiguration = _dashboardConfiguration
            };

            return View(compositeViewModel);
        }
    }
}
