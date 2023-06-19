using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    internal enum PositionMap
    {
        TwoDimension,
        ThreeDimension
    }
}
