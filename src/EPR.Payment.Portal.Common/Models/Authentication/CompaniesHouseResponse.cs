using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class CompaniesHouseResponse
    {
        public OrganisationDto? Organisation { get; set; }

        public bool AccountExists => AccountCreatedOn is not null;

        public DateTimeOffset? AccountCreatedOn { get; set; }
    }
}