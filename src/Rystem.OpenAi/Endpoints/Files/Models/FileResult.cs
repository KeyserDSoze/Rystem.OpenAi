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
        /// <summary>
        /// When the object is deleted, this attribute is used in the Delete file operation
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
        [JsonPropertyName("file")]
        public FilesDataResult? File { get; set; }
    }
    public sealed class FilesDataResult
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
    }

    public class FilePartResult
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("upload_id")]
        public string? UploadId { get; set; }
    }
}
