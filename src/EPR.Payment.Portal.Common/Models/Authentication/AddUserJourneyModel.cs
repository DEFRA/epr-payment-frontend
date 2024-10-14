using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Models.Authentication
{
    [ExcludeFromCodeCoverage]
    public class AddUserJourneyModel
    {
        public string Email { get; set; }

        public string UserRole { get; set; }
    }
}