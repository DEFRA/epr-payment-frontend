using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    [Route("", Name = RouteNames.GovPay.PaymentError)]
    public class ErrorController(IOptions<DashboardConfiguration> dashboardConfiguration) : Controller
    {
        private readonly DashboardConfiguration _dashboardConfiguration = dashboardConfiguration?.Value
            ?? throw new ArgumentNullException(nameof(dashboardConfiguration));

        [HttpGet]
        public IActionResult Index() => View(_dashboardConfiguration);
    }

}
