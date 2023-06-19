using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class WeatherResponseModel
    {
        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
