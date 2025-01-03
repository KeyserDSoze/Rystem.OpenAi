using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Files
{
    public sealed class FilePartResult : UnixTimeBase
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("upload_id")]
        public string? UploadId { get; set; }
    }
}
