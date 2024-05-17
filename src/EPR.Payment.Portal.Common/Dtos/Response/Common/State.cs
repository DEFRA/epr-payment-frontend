using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class State
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("finished")]
        public bool Finished { get; set; }
    }
}
