using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Files
{
    public sealed class FileDataResult : UnixTimeBase
    {
        /// <summary>
        /// Unique id for this file, so that it can be referenced in other operations
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// Object type
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        /// <summary>
        /// The name of the file
        /// </summary>
        [JsonPropertyName("filename")]
        public string? Name { get; set; }
        /// <summary>
        /// What is the purpose of this file, fine-tune, search, etc
        /// </summary>
        [JsonPropertyName("purpose")]
        public string? Purpose { get; set; }
        //todo: purpose result has to be the purpose enum
        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        [JsonPropertyName("bytes")]
        public long Bytes { get; set; }
    }
}
