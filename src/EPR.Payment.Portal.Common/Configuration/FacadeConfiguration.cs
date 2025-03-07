﻿using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class FacadeConfiguration
    {
        public static string SectionName => "PaymentFacade";

        public FacadeService FacadeService { get; set; } = new FacadeService();
    }

    public class FacadeService
    {
        public string? Url { get; set; }
        public string? EndPointName { get; set; }
        public string? HttpClientName { get; set; }
        public string? DownstreamScope { get; set; }
    }
}
