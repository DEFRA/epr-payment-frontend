using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Options
{
    [ExcludeFromCodeCoverage]
    public class GlobalVariables
    {
        public required string BasePath { get; set; }

        public bool UseLocalSession { get; set; }
    }
}