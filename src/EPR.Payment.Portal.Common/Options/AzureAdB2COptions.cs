using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Options
{
    [ExcludeFromCodeCoverage]
    public class AzureAdB2COptions
    {
        public const string ConfigSection = "AzureAdB2C";
        public required string SignedOutCallbackPath { get; set; }
    }
}