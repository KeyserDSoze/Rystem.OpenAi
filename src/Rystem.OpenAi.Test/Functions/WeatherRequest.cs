using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class WeatherRequest
    {
        [JsonPropertyName("location")]
        [JsonRequired]
        [JsonPropertyDescription("The city and state, e.g. San Francisco, CA")]
        public string Location { get; set; }
        [JsonPropertyName("unit")]
        [JsonPropertyDescription("Unit Measure of temperature. e.g. Celsius or Fahrenheit")]
        [JsonPropertyAllowedValues]
        public string Unit { get; set; }
    }
}
