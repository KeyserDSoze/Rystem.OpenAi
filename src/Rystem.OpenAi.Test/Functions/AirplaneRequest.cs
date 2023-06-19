using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class AirplaneRequest
    {
        [JsonPropertyDescription("Departure and arrival position for flight")]
        [JsonRequired]
        public AirplanePosition Position { get; set; }
        [JsonPropertyDescription("Maximum price allowed by the client")]
        [JsonPropertyRange(10, 560, true)]
        public int MaximumPrice { get; set; }
    }
}
