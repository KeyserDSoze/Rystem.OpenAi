using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class AssistantRequest : IOpenAiRequest
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("created_at")]
        public long? CreatedAt { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonIgnore]
        public StringBuilder? InstructionsBuilder { get; set; }
        [JsonPropertyName("instructions")]
        public string? Instructions { get => InstructionsBuilder?.ToString(); set => InstructionsBuilder = new(value); }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonPropertyName("response_format")]
        public object? ResponseFormat { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("tools")]
        public List<object>? Tools { get; set; }
        [JsonPropertyName("tool_resources")]
        public object? ToolResources { get; set; }
    }
}
