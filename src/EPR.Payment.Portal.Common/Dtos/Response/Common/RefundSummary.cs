using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class RefundSummary
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("amount_available")]
        public int AmountAvailable { get; set; }

        [JsonProperty("amount_submitted")]
        public int AmountSubmitted { get; set; }
    }
}
