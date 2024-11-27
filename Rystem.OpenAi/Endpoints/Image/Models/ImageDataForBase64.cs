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
        public System.Drawing.Image? ConvertToImage()
        {
            if (!string.IsNullOrWhiteSpace(Base64Value))
            {
                var bytes = Convert.FromBase64String(Base64Value);
                using var memoryStream = new MemoryStream(bytes);
                var image = System.Drawing.Image.FromStream(memoryStream);
                return image;
            }
            return default;
        }
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
