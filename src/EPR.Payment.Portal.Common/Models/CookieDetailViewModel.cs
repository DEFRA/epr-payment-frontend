namespace EPR.Payment.Portal.Common.Models
{
    public class CookieDetailViewModel
    {
        public bool CookiesAccepted { get; set; }

        public bool ShowAcknowledgement { get; set; }

        public required string CookiePolicyCookieName { get; set; }

        public required string SessionCookieName { get; set; }

        public required string AntiForgeryCookieName { get; set; }

        public required string TsCookieName { get; set; }

        public required string AuthenticationCookieName { get; set; }

        public required string TempDataCookieName { get; set; }

        public required string B2CCookieName { get; set; }

        public required string CorrelationCookieName { get; set; }

        public required string OpenIdCookieName { get; set; }

        public required string GoogleAnalyticsDefaultCookieName { get; set; }

        public required string GoogleAnalyticsAdditionalCookieName { get; set; }

        public string? ReturnUrl { get; set; }
    }
}