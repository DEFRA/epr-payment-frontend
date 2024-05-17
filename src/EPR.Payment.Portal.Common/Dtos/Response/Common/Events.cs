using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class Events
    {
        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("method")]
        public string? Method { get; set; }
    }
}
