using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class NextUrlPost
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("params")]
        public Params? Params { get; set; }

        [JsonProperty("href")]
        public string? Href { get; set; }

        [JsonProperty("method")]
        public string? Method { get; set; }
    }
}
