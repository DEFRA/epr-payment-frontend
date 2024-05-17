﻿using Newtonsoft.Json;

namespace EPR.Payment.Portal.Common.Dtos.Response.Common
{
    public class Links
    {
        [JsonProperty("self")]
        public Self? Self { get; set; }

        [JsonProperty("next_url")]
        public NextUrl? NextUrl { get; set; }

        [JsonProperty("next_url_post")]
        public NextUrlPost? NextUrlPost { get; set; }

        [JsonProperty("events")]
        public Events? Events { get; set; }

        [JsonProperty("refunds")]
        public Refunds? Refunds { get; set; }

        [JsonProperty("cancel")]
        public Cancel? Cancel { get; set; }
    }
}
