namespace EPR.Payment.Portal.Common.Configuration
{
    public class GovPayErrorConfiguration
    {
        public static string SectionName => "GovPayError";

        public GovPayErrorService TryPaymentAgainUrl { get; set; } = new GovPayErrorService();
    }

    public class GovPayErrorService
    {
        public string? Url { get; set; }
        public string? Description { get; set; }
    }
}
