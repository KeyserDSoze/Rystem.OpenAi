using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageResult : ApiBaseResponse
    {
        [JsonPropertyName("data")]
        public List<ImageData>? Data { get; set; }
    }
}
