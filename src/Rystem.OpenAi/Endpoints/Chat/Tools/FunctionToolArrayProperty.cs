using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class FunctionToolArrayProperty : FunctionToolProperty
    {
        [JsonPropertyName("items")]
        public FunctionToolProperty? Items { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
