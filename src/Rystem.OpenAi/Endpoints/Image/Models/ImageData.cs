using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageData
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("revised_prompt")]
        public string? RevisedPrompt { get; set; }
    }
}
