using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class Metadata
    {
        [JsonProperty("ledger_code")]
        public string? LedgerCode { get; set; }

        [JsonProperty("internal_reference_number")]
        public int InternalReferenceNumber { get; set; }
    }
}
