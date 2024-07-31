using EPR.Payment.Portal.Common.Enums;
using EPR.Payment.Portal.Helpers;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPayCallbackController : Controller
    {
        private readonly IPaymentsService _paymentsServices;

        public GovPayCallbackController(IPaymentsService paymentsServices)
        {
            _paymentsServices = paymentsServices;
        }
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var viewModel = await _paymentsServices.CompletePaymentAsync(id, cancellationToken);
                string controllerName = string.Concat("Payment", ((PaymentStatus)viewModel.Status).ToString());
                return RedirectToAction("Index", controllerName, viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "PaymentError", new {message = ex.Message});
            }
        }
    }
}
