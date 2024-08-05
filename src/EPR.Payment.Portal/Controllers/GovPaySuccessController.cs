using EPR.Payment.Portal.Common.Constants;
using EPR.Payment.Portal.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers
{
    public class GovPaySuccessController : Controller
    {
        public IActionResult Index(CompletePaymentViewModel? completePaymentResponseViewModel)
        {
            if (completePaymentResponseViewModel == null)
            {
                return RedirectToAction("Index", "Error", new { message = ExceptionMessages.ErrorInvalidViewModel });
            }
            ViewData["reference"] = completePaymentResponseViewModel.Reference;
            return View();
        }
    }
}
