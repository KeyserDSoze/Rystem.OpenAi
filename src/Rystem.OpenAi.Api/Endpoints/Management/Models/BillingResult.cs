using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public class BillingResult
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }
        [JsonPropertyName("daily_costs")]
        public List<DailyCosts> DailyCosts { get; set; }
        [JsonPropertyName("total_usage")]
        public float TotalUsage { get; set; }
    }

}
