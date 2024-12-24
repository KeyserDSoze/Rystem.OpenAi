using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Files
{
    /// <summary>
    /// Represents a single file used with the OpenAI Files endpoint.  Files are used to upload and manage documents that can be used with features like Fine-tuning.
    /// </summary>
    public sealed class FileResult
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
        /// <summary>
        /// Timestamp for the creation time of this file
        /// </summary>
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("file")]
        public FileDataResult? File { get; set; }
    }
}
