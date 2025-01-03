using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepImage
    {
        [JsonPropertyName("file_id")]
        public string? FileId { get; set; }
    }
}
