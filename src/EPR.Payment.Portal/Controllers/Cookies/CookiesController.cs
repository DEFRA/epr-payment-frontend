using EPR.Payment.Portal.Common.Models;
using EPR.Payment.Portal.Common.Options;
using EPR.Payment.Portal.Constants;
using EPR.Payment.Portal.Infrastructure;
using EPR.Payment.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CookieOptions = EPR.Payment.Portal.Common.Options.CookieOptions;

namespace EPR.Payment.Portal.Controllers.Cookies
{
    [AllowAnonymous]
    [Route("cookies")]
    public class CookiesController : Controller
    {
        private readonly ICookieService _cookieService;
        private readonly CookieOptions _eprCookieOptions;
        private readonly GoogleAnalyticsOptions _googleAnalyticsOptions;

        public CookiesController(
            ICookieService cookieService,
            IOptions<CookieOptions> eprCookieOptions,
            IOptions<GoogleAnalyticsOptions> googleAnalyticsOptions)
        {
            _cookieService = cookieService;
            _eprCookieOptions = eprCookieOptions.Value;
            _googleAnalyticsOptions = googleAnalyticsOptions.Value;
        }

        [Route("cookies", Name = RouteNames.Cookies.Detail)]
        public IActionResult Detail(string returnUrl, bool? cookiesAccepted = null)
        {
            // Validate the model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate the return URL
            if (string.IsNullOrWhiteSpace(returnUrl) || !IsValidReturnUrl(returnUrl))
            {
                // Log or handle invalid input as necessary
                return BadRequest("Invalid return URL.");
            }

            // Allowed return URLs
            var allowedBackValues = new List<string>
            {
                "/report-data",
                "/create-account",
                "/manage-account"
            };

            // Validate the return URL
            var validBackLink = !string.IsNullOrWhiteSpace(returnUrl) && allowedBackValues.Exists(a => returnUrl.StartsWith(a));
            string returnUrlAddress = validBackLink ? returnUrl : Url.Content("~/");

            // Check if cookies have been accepted
            var hasUserAcceptedCookies = cookiesAccepted ?? _cookieService.HasUserAcceptedCookies(Request.Cookies);

            // Create the view model
            var cookieViewModel = new CookieDetailViewModel
            {
                SessionCookieName = _eprCookieOptions.SessionCookieName,
                CookiePolicyCookieName = _eprCookieOptions.CookiePolicyCookieName,
                AntiForgeryCookieName = _eprCookieOptions.AntiForgeryCookieName,
                GoogleAnalyticsDefaultCookieName = _googleAnalyticsOptions.DefaultCookieName,
                GoogleAnalyticsAdditionalCookieName = _googleAnalyticsOptions.AdditionalCookieName,
                AuthenticationCookieName = _eprCookieOptions.AuthenticationCookieName,
                TsCookieName = _eprCookieOptions.TsCookieName,
                TempDataCookieName = _eprCookieOptions.TempDataCookie,
                B2CCookieName = _eprCookieOptions.B2CCookieName,
                CorrelationCookieName = _eprCookieOptions.CorrelationCookieName,
                OpenIdCookieName = _eprCookieOptions.OpenIdCookieName,
                CookiesAccepted = hasUserAcceptedCookies,
                ReturnUrl = returnUrlAddress,
                ShowAcknowledgement = cookiesAccepted != null
            };

            // Set view bag properties
            ViewBag.BackLinkToDisplay = returnUrlAddress;
            ViewBag.CurrentPage = returnUrlAddress;

            return View(cookieViewModel);
        }

        [HttpPost]
        [Route("cookies", Name = RouteNames.Cookies.Detail)]
        public IActionResult Detail(string returnUrl, string cookiesAccepted)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                // Log or handle invalid model state as necessary
                return BadRequest("Invalid data provided.");
            }

            // Update cookie acceptance
            _cookieService.SetCookieAcceptance(cookiesAccepted == CookieAcceptance.Accept, Request.Cookies, Response.Cookies);
            TempData[CookieAcceptance.CookieAcknowledgement] = cookiesAccepted;

            return Detail(returnUrl, cookiesAccepted == CookieAcceptance.Accept);
        }

        [HttpPost]
        [Route("update-cookie-acceptance", Name = RouteNames.Cookies.UpdateAcceptance)]
        public IActionResult UpdateAcceptance(string returnUrl, string cookies)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            // Update cookie acceptance
            _cookieService.SetCookieAcceptance(cookies == CookieAcceptance.Accept, Request.Cookies, Response.Cookies);
            TempData[CookieAcceptance.CookieAcknowledgement] = cookies;

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [Route("acknowledge-cookie-acceptance", Name = RouteNames.Cookies.AcknowledgeAcceptance)]
        public IActionResult AcknowledgeAcceptance(string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input.");
            }

            if (string.IsNullOrWhiteSpace(returnUrl) || !IsValidReturnUrl(returnUrl))
            {
                return LocalRedirect("~/"); // Redirect to a default page or handle as necessary
            }

            return LocalRedirect(returnUrl);
        }

        private bool IsValidReturnUrl(string returnUrl)
        {
            return returnUrl.StartsWith('/');
        }
    }
}