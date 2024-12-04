using EPR.Payment.Portal.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers.Culture
{
    [AllowAnonymous]
    public class CultureController : Controller
    {
        [HttpGet]
        public IActionResult UpdateCulture(string culture, string returnUrl)
        {
            HttpContext.Session.SetString(Language.SessionLanguageKey, culture);

            // Sanitize the returnUrl to ensure it includes the base path
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/payment/"; // Default to base path
            }

            return LocalRedirect(returnUrl);
        }
    }
}
