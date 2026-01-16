using EPR.Payment.Portal.Common.Configuration;

namespace EPR.Payment.Portal.Common.Models
{
    public class CompositeViewModel
    {
        public CompletePaymentViewModel CompletePaymentViewModel { get; set; } = null!;
        public DashboardConfiguration DashboardConfiguration { get; set; } = null!;
        public string? OrganisationName { get; set; }
    }
}
