using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class OrganisationDto
    {
        public string? Name { get; set; }

        public string? TradingName { get; set; }

        public string? RegistrationNumber { get; set; }

        public string? CompaniesHouseNumber { get; set; }

        public AddressDto? RegisteredOffice { get; set; }

        public OrganisationDataDto? OrganisationData { get; set; }
    }
}