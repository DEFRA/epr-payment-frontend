using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.Payment.Portal.Common.Configuration
{
    public class DashboardConfiguration
    {
        public static string SectionName => "Dashboard";

        public Service MenuUrl { get; set; } = new Service();
        public Service BackUrl { get; set; } = new Service();
        public Service FeedbackUrl { get; set; } = new Service();
        public Service OfflinePaymentUrl { get; set; } = new Service();
    }

    public class Service
    {
        public string? Url { get; set; }
        public string? Description { get; set; }
    }
}
