using EPR.Payment.Portal.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Controllers.Culture
{
    [AllowAnonymous]
    [ExcludeFromCodeCoverage]
    public class CultureController : Controller
    {
        [HttpGet]
        [Route("culture")]
        public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
        {
            HttpContext.Session.SetString(Language.SessionLanguageKey, culture);
            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";
            return LocalRedirect(returnUrl);
        }
    }
}
