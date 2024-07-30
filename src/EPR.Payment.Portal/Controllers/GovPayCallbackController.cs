using EPR.Payment.Portal.Common.Enums;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPayCallbackController : Controller
    {
        private readonly IPaymentsServices _paymentsServices;

        public GovPayCallbackController(IPaymentsServices paymentsServices)
        {
            _paymentsServices = paymentsServices;
        }
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken)
        {
            var viewModel = await _paymentsServices.CompletePaymentAsync(id, cancellationToken);
            return RedirectToAction(
                    "Index",
                    "Payment" + ((PaymentStatus)viewModel.Status).ToString(),
                        viewModel);
        }
    }
}
