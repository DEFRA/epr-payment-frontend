using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Enums;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPayCallbackController : Controller
    {
        private readonly IPaymentsService _paymentsService; 
        private readonly ILogger _logger;

        public GovPayCallbackController(IPaymentsService paymentsService, ILogger<GovPayCallbackController> logger)
        {
            _paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError(ExceptionMessages.ErrorExternalPaymentIdEmpty);
                return RedirectToAction("Index", "PaymentError", new { message = ExceptionMessages.ErrorExternalPaymentIdEmpty });
            }

            try
            {
                var viewModel = await _paymentsService.CompletePaymentAsync(id, cancellationToken);
                string controllerName = (PaymentStatus)viewModel.Status == PaymentStatus.Success ? "PaymentSuccess" : "GenericFailure";
                return RedirectToAction("Index", controllerName, viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing payment for ID {PaymentId}", id);
                return RedirectToAction("Index", "PaymentError", new {message = ex.Message});
            }
        }
    }
}
