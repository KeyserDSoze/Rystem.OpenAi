using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageResultForBase64
    {
        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public long? CreatedUnixTime { get; set; }
        [JsonPropertyName("data")]
        public List<ImageDataForBase64>? Data { get; set; }
    }
}
