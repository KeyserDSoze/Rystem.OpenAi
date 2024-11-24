using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public class DailyCosts
    {
        [JsonPropertyName("timestamp")]
        public float Timestamp { get; set; }
        public DateTime Day => new DateTime((long)Timestamp);
        [JsonPropertyName("line_items")]
        public List<CostItems>? Items { get; set; }
    }

}
