using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPaySuccessController : Controller
    {
        private readonly DashboardConfiguration _dashboardConfiguration;
        public GovPaySuccessController(IOptions<DashboardConfiguration> dashboardConfiguration)
        {
            _dashboardConfiguration = dashboardConfiguration.Value ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
        }
        public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
        {
            if (!ModelState.IsValid || completePaymentResponseViewModel == null)
            {
                return RedirectToAction("Index", "Error", new { message = ExceptionMessages.ErrorInvalidViewModel });
            }
            var compositeViewModel = new CompositeViewModel() { completePaymentViewModel = completePaymentResponseViewModel, dashboardConfiguration = _dashboardConfiguration };

            return View(compositeViewModel);
        }
    }
}
