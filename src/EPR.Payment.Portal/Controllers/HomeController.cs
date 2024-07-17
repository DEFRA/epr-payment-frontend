using EPR.Payment.Portal.Common.Configuration;
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
            return View(_dashboardConfiguration);
        }
    }
}
