using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace EPR.Payment.Portal.ViewComponents
{
    public class CookieBannerViewComponent : ViewComponent
    {
        private readonly EPR.Payment.Portal.Common.Options.CookieOptions _options;

        public CookieBannerViewComponent(IOptions<EPR.Payment.Portal.Common.Options.CookieOptions> options)
        {
            _options = options.Value;
        }

        public IViewComponentResult Invoke(string returnUrl)
        {
            var consentCookie = Request.Cookies[_options.CookiePolicyCookieName];

            var cookieAcknowledgement = TempData[CookieAcceptance.CookieAcknowledgement]?.ToString();

            var dontShowBanner = ViewContext.RouteData.Values["controller"]?.ToString() == "Cookies";

            var cookieBannerModel = new CookieBannerModel
            {
                CurrentPage = Request.Path,
                ShowBanner = !dontShowBanner && cookieAcknowledgement == null && consentCookie == null,
                ShowAcknowledgement = !dontShowBanner && cookieAcknowledgement != null,
                AcceptAnalytics = cookieAcknowledgement == CookieAcceptance.Accept,
                ReturnUrl = $"{returnUrl}{Request.QueryString}",
            };

            return View(cookieBannerModel);
        }
    }
}