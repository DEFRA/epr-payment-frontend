using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class SessionOptions
    {
        public const string ConfigSection = "Session";

        public int IdleTimeoutMinutes { get; set; }
    }
}
