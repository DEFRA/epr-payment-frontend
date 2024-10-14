using EPR.Payment.Portal.Common.Models.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Sessions
{
    [ExcludeFromCodeCoverage]
    public record CompaniesHouseSession
    {
        public CompaniesHouseResponse CompaniesHouseData { get; set; }
    }
}