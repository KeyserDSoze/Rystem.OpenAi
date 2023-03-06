using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageCreateRequest : IOpenAiRequest
    {
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }
        [JsonPropertyName("n")]
        public int NumberOfResults { get; set; }
        [JsonPropertyName("size")]
        public string? Size { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
        [JsonPropertyName("response_format")]
        public string? ResponseFormat { get; set; }
        [JsonIgnore]
        public string? ModelId { get; set; }
    }
}
