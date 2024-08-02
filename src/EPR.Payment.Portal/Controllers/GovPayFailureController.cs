using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Dtos.Request;
using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPayFailureController : Controller
    {
        private readonly IPaymentsService _paymentsService;
        private readonly DashboardConfiguration _dashboardConfiguration;
        private readonly ILogger _logger;

        public GovPayFailureController(IPaymentsService paymentsService, IOptions<DashboardConfiguration> dashboardConfiguration, ILogger<GovPayFailureController> logger)
        {
            _paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            _dashboardConfiguration = dashboardConfiguration.Value ?? throw new ArgumentNullException(nameof(dashboardConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
        {
            if (completePaymentResponseViewModel == null)
            {
                return RedirectToAction("Index", "Error", new { message = ExceptionMessages.ErrorInvalidViewModel });
            }
            ViewData["amount"] = completePaymentResponseViewModel.Amount / 100;

            var compositeViewModel = new CompositeViewModel() { completePaymentViewModel = completePaymentResponseViewModel, dashboardConfiguration = _dashboardConfiguration };

            return View(compositeViewModel);
        }

        public async Task<IActionResult> InitiatePayment(PaymentRequestDto? request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError(ExceptionMessages.ErrorInvalidPaymentRequestDto);
                return RedirectToAction("Index", "Error", new { message = ExceptionMessages.ErrorInvalidPaymentRequestDto });
            }

            try
            {
                var responseContent = await _paymentsService.InitiatePaymentAsync(request, cancellationToken);
                return Content(responseContent, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.ErrorInitiatePayment);
                return RedirectToAction("Index", "Error", new { message = ex.Message });
            }
        }
    }
}
