using EPR.Payment.Portal.Common.Models.Request;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentsService _paymentsService;

        public PaymentsController(IPaymentsService paymentsService)
        {
            _paymentsService = paymentsService;
        }

        [HttpGet("{paymentId}/Status")]
        public async Task<IActionResult> PaymentStatus(string? paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                return BadRequest("Invalid 'paymentId' parameter provided");
            }

            var PaymentStatusResponseVm = await _paymentsService.GetPaymentStatus(paymentId);

            return View(PaymentStatusResponseVm);
        }

        [HttpPost("{paymentId}/status")]
        public async Task<IActionResult> PaymentStatus(string paymentId, PaymentStatusInsertRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PaymentStatus", new { paymentId });
            }

            await _paymentsService.InsertPaymentStatus(paymentId, vm);

            return RedirectToAction("Index", "Home");
        }
    }
}
