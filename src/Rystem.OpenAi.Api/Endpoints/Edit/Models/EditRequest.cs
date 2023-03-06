using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Edit
{
    public sealed class EditRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
        [JsonPropertyName("input")]
        public string? Input { get; set; }
        [JsonPropertyName("instruction")]
        public string? Instruction { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("n")]
        public int? NumberOfChoicesPerPrompt { get; set; }
    }
}
