using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Infrastructure;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    [Route("GovPayFailure", Name = RouteNames.GovPay.Paymentfailure)]
    public class GovPayFailureController : Controller
    {
        private readonly IPaymentsService _paymentsService;
        private readonly DashboardConfiguration _dashboardConfiguration;
        private readonly ILogger<GovPayFailureController> _logger;

        public GovPayFailureController(
            IPaymentsService paymentsService,
            IOptions<DashboardConfiguration> dashboardConfiguration,
            ILogger<GovPayFailureController> logger)
        {
            _paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            _dashboardConfiguration = dashboardConfiguration?.Value ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
        {
            if (!ModelState.IsValid || completePaymentResponseViewModel is null)
            {
                return RedirectToRoute(RouteNames.GovPay.PaymentError,
                    new { message = ExceptionMessages.ErrorInvalidViewModel });
            }

            ViewData["amount"] = completePaymentResponseViewModel.Amount / 100;

            var compositeViewModel = new CompositeViewModel
            {
                completePaymentViewModel = completePaymentResponseViewModel,
                dashboardConfiguration = _dashboardConfiguration
            };

            return View(compositeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PaymentRequestDto? request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || request is null)
            {
                _logger.LogError(ExceptionMessages.ErrorInvalidPaymentRequestDto);
                return RedirectToRoute(RouteNames.GovPay.PaymentError,
                    new { message = ExceptionMessages.ErrorInvalidPaymentRequestDto });
            }

            try
            {
                var responseContent = await _paymentsService.InitiatePaymentAsync(request, cancellationToken);
                return Content(responseContent, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.ErrorInitiatePayment);
                return RedirectToRoute(RouteNames.GovPay.PaymentError,
                    new { message = ex.Message });
            }
        }
    }

}
