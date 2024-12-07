using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageDataForBase64
    {
        [JsonPropertyName("b64_json")]
        public string? Base64Value { get; set; }
        [JsonPropertyName("revised_prompt")]
        public string? RevisedPrompt { get; set; }
        public MemoryStream? ConvertToStream()
        {
            if (!string.IsNullOrWhiteSpace(Base64Value))
            {
                var bytes = Convert.FromBase64String(Base64Value);
                var memoryStream = new MemoryStream(bytes);
                return memoryStream;
            }
            return default;
        }
    }
}
