using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class RemoveUserJourneyModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid PersonId { get; set; }
    }
}