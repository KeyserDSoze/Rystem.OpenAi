using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    /// <summary>
    /// Represents a language model
    /// </summary>
    public sealed class ModelResult : UnixTimeBaseResponse
    {
        /// <summary>
        /// The id/name of the model
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        /// <summary>
        /// The owner of this model.  Generally "openai" is a generic OpenAI model, or the organization if a custom or finetuned model.
        /// </summary>
        [JsonPropertyName("owned_by")]
        public string? OwnedBy { get; set; }
    }
}
