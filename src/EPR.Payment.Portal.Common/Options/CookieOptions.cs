using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class CookieOptions
    {
        public const string ConfigSection = "Cookie";
        public int CookiePolicyDurationInMonths { get; set; }
        public required string SessionCookieName { get; set; }
        public required string CookiePolicyCookieName { get; set; }
        public required string AntiForgeryCookieName { get; set; }
        public required string TsCookieName { get; set; }
        public required string CorrelationCookieName { get; set; }
        public required string OpenIdCookieName { get; set; }
        public required string B2CCookieName { get; set; }
        public required string AuthenticationCookieName { get; set; }
        public int AuthenticationExpiryInMinutes { get; set; }
        public required string TempDataCookie { get; set; }
    }
}