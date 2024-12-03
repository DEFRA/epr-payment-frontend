using EPR.Payment.Portal.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Controllers.Culture
{
    [AllowAnonymous]
    public class CultureController (ILogger<CultureController> logger) : Controller
    {
        private readonly ILogger<CultureController> _logger = logger;

        [HttpGet]
        [Route("culture")]
        public LocalRedirectResult UpdateCulture(string culture, string returnUrl)
        {
            try
            {
                _logger.LogError("Inside Updateculture");
                HttpContext.Session.SetString(Language.SessionLanguageKey, culture);
                return LocalRedirect(returnUrl);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "UpdateCulture Error Message: {Exception}",  ex.Message);
            }
            return LocalRedirect(returnUrl);
        }
    }
}
