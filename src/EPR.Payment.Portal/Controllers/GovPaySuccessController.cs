using EPR.Common.Authorization.Extensions;
using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers;

[Route("GovPaySuccess", Name = RouteNames.GovPay.PaymentSuccess)]
public class GovPaySuccessController(IOptions<DashboardConfiguration> dashboardConfiguration) : Controller
{
    private readonly DashboardConfiguration _dashboardConfiguration = dashboardConfiguration?.Value
                                                                      ?? throw new ArgumentNullException(nameof(dashboardConfiguration));

    [HttpGet]
    public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
    {
        if (!ModelState.IsValid || completePaymentResponseViewModel is null)
        {
            return RedirectToRoute(RouteNames.GovPay.PaymentError, new { message = ExceptionMessages.ErrorInvalidViewModel });
        }

        var userData = User.GetUserData();
        var organisationName = userData.Organisations.FirstOrDefault()?.Name;

        var compositeViewModel = new CompositeViewModel
        {
            CompletePaymentViewModel = completePaymentResponseViewModel,
            DashboardConfiguration = _dashboardConfiguration,
            OrganisationName = organisationName
        };

        return View(compositeViewModel);
    }
}