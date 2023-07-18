using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class CarRequestModel
    {
        [JsonPropertyName("plate")]
        [JsonPropertyDescription("The plate of the car.")]
        [JsonRequired]
        public string Plate { get; set; }
        [JsonPropertyName("dimension")]
        [JsonPropertyDescription("Searching with position in 2 dimensions or 3 dimensions.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonRequired]
        public PositionMap Dimension { get; set; }
    }
}
