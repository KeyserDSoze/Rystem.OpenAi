using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Management
{
    public class CostItems
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("cost")]
        public decimal Cost { get; set; }
    }

}
