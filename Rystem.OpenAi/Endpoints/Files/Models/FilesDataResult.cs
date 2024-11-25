using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Files
{
    public sealed class FilesDataResult
    {
        [JsonPropertyName("data")]
        public List<FileResult>? Data { get; set; }
        /// <summary>
        /// Object type
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }
    }
}
