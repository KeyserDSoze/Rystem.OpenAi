using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public sealed class FunctionTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
        [JsonPropertyName("parameters")]
        public FunctionToolMainProperty Parameters { get; set; } = null!;
        [JsonPropertyName("strict")]
        public bool? Strict { get; set; }
    }
}
