using EPR.Payment.Portal.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPayStatusController : Controller
    {
        private readonly GovPayErrorConfiguration _govPayErrorConfiguration;

        public GovPayStatusController(IOptions<GovPayErrorConfiguration> govPayErrorConfiguration)
        {
            _govPayErrorConfiguration = govPayErrorConfiguration.Value ?? throw new ArgumentNullException(nameof(govPayErrorConfiguration));
        }
        public IActionResult Failure(double? PaymentAmount)
        {
            ViewData["PaymentAmount"] = PaymentAmount;
            return View(_govPayErrorConfiguration);
        }
    }
}
