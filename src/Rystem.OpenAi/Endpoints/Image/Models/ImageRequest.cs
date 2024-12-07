using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageRequest : IOpenAiRequest
    {
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }
        [JsonPropertyName("n")]
        public int NumberOfResults { get; set; }
        [JsonPropertyName("size")]
        public string? Size { get; set; }
        [JsonPropertyName("quality")]
        public string? Quality { get; set; }
        [JsonPropertyName("style")]
        public string? Style { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
        [JsonPropertyName("response_format")]
        public string? ResponseFormat { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
    }
}
