using EPR.Payment.Portal.Common.Models.Authentication;

namespace EPR.Payment.Portal.Sessions
{
    public class AccountManagementSession
    {
        public List<string> Journey { get; set; } = new();

        public string? OrganisationName { get; set; }

        public string InviteeEmailAddress { get; set; } = default!;

        public string RoleKey { get; set; } = default!;

        public RemoveUserJourneyModel? RemoveUserJourney { get; set; }

        public AddUserJourneyModel? AddUserJourney { get; set; }

        public EndpointResponseStatus? RemoveUserStatus { get; set; }

        public EndpointResponseStatus? AddUserStatus { get; set; }

    }
}