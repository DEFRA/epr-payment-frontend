using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class AccountsFacadeApiOptions
    {
        public const string ConfigSection = "AccountsFacadeAPI";
        public required string BaseEndpoint { get; set; }
        public required string DownstreamScope { get; set; }
    }
}
