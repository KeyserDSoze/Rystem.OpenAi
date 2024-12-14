using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Files
{
    public sealed class FilePartialStartRequest
    {
        [JsonPropertyName("purpose")]
        public string? Purpose { get; set; }

        [JsonPropertyName("filename")]
        public string? FileName { get; set; }

        [JsonPropertyName("bytes")]
        public long? Bytes { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }
}
