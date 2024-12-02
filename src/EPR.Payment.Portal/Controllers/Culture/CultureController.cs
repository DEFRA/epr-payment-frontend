using EPR.Payment.Portal.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPR.Payment.Portal.Controllers.Culture
{
    [AllowAnonymous]
    public class CultureController : Controller
    {
        [HttpGet]
        [Route("culture")]
        public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
        {
            HttpContext.Session.SetString(Language.SessionLanguageKey, culture);
            return LocalRedirect(returnUrl);
        }
    }
}
