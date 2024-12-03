using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class GoogleAnalyticsOptions
    {
        public const string ConfigSection = "GoogleAnalytics";

        public required string CookiePrefix { get; set; }

        public required string MeasurementId { get; set; }

        public required string TagManagerContainerId { get; set; }

        public string DefaultCookieName => CookiePrefix;

        public string AdditionalCookieName => $"{CookiePrefix}_{MeasurementId}";
    }
}
