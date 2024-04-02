using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    /// <summary>
    /// Represents a language model
    /// </summary>
    public sealed class Model : OpenAiObject
    {
        /// <summary>
        /// The id/name of the model
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// The owner of this model.  Generally "openai" is a generic OpenAI model, or the organization if a custom or finetuned model.
        /// </summary>
        [JsonPropertyName("owned_by")]
        public string? OwnedBy { get; set; }
        [JsonIgnore]
        public DateTime? Created => CreatedUnixTime.HasValue ? (DateTime?)(DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime.Value).DateTime) : null;
        /// <summary>
        /// The time when the model was created in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public long? CreatedUnixTime { get; set; }
    }
}
