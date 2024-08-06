using EPR.Payment.Portal.Common.Configuration;

namespace EPR.Payment.Portal.Common.Models
{
    public class CompositeViewModel
    {
        public CompletePaymentViewModel completePaymentViewModel { get; set; } = null!;
        public DashboardConfiguration dashboardConfiguration { get; set; } = null!;
    }
}
