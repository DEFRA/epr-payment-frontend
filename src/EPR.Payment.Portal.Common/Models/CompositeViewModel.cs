using EPR.Payment.Portal.Common.Configuration;
using EPR.Payment.Portal.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.Common.Models
{
    public class CompositeViewModel
    {
        public CompletePaymentViewModel completePaymentViewModel { get; set; } = null!;
        public DashboardConfiguration dashboardConfiguration { get; set; } = null!;
    }
}
