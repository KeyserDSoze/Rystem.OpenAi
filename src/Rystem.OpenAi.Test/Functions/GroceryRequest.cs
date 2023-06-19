using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class GroceryRequest
    {
        [JsonPropertyDescription("A list of vegetables to buy.")]
        [JsonPropertyName("vegetables")]
        public string[] Vegetables { get; set; }
        [JsonPropertyDescription("A list of meet to buy.")]
        [JsonPropertyName("meat")]
        public List<string> Meat { get; set; }
        [JsonPropertyDescription("A list of fish to buy.")]
        [JsonPropertyName("fish")]
        public string[] Fish { get; set; }
    }
}
