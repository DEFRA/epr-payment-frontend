using System.Diagnostics.CodeAnalysis;

namespace EPR.Payment.Portal.Common.Configuration
{
    [ExcludeFromCodeCoverage]
    public class FacadeConfiguration
    {
        public static string SectionName => "PaymentFacade";

        public FacadeService FacadeService { get; set; } = new ();

        public FacadeServiceV2 FacadeServiceV2 { get; set; } = new ();
    }

    public class FacadeService
    {
        public string? Url { get; set; }
        public string? EndPointName { get; set; }
        public string? HttpClientName { get; set; }
        public string? DownstreamScope { get; set; }
    }

#pragma warning disable S2094
    public class FacadeServiceV2: FacadeService
#pragma warning restore S2094
    {
    }
}
