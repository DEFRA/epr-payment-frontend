using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Enums;
using EPR.Payment.Portal.Infrastructure;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EPR.Payment.Portal.Controllers
{
    [Route("GovPayCallback", Name = RouteNames.GovPay.GovPayCallback)]
    public class GovPayCallbackController : Controller
    {
        private readonly IPaymentsService _paymentsService;
        private readonly ILogger<GovPayCallbackController> _logger;

        public GovPayCallbackController(
            IPaymentsService paymentsService,
            ILogger<GovPayCallbackController> logger)
        {
            _paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken)
        {


            if (!ModelState.IsValid || id == Guid.Empty)
            {
                _logger.LogError("{Message}", ExceptionMessages.ErrorExternalPaymentIdEmpty);
                return RedirectToRoute(RouteNames.GovPay.PaymentError, new { message = ExceptionMessages.ErrorExternalPaymentIdEmpty });
            }

            try
            {
                var viewModel = await _paymentsService.CompletePaymentAsync(id, cancellationToken);

                var routeName = viewModel.Status switch
                {
                    PaymentStatus.Success => RouteNames.GovPay.PaymentSuccess,
                    _ => RouteNames.GovPay.Paymentfailure
                };

                return RedirectToRoute(routeName, viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing payment for ID {PaymentId}", id);
                return RedirectToRoute(RouteNames.GovPay.PaymentError, new { message = ex.Message });
            }
        }
    }

}
