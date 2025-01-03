using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageResultForBase64 : UnixTimeBaseResponse
    {
        [JsonPropertyName("data")]
        public List<ImageDataForBase64>? Data { get; set; }
    }
}
