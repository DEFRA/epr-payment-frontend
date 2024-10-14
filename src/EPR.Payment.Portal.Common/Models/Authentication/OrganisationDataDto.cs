using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class OrganisationDataDto
    {
        public DateTime DateOfCreation { get; set; }

        public string? Status { get; set; }

        public string? Type { get; set; }
    }
}