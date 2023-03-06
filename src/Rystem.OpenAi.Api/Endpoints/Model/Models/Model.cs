using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Models
{
    /// <summary>
    /// Represents a language model
    /// </summary>
    public sealed class Model
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
        /// <summary>
        /// The type of object. Should always be 'model'.
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonIgnore]
        public DateTime? Created => CreatedUnixTime.HasValue ? (DateTime?)(DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime.Value).DateTime) : null;
        /// <summary>
        /// The time when the model was created in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public long? CreatedUnixTime { get; set; }
        /// <summary>
        /// Permissions for use of the model
        /// </summary>
        [JsonPropertyName("permission")]
        public List<Permissions> Permission { get; set; } = new List<Permissions>();
        /// <summary>
        /// Currently (2023-01-27) seems like this is duplicate of <see cref="Id"/> but including for completeness.
        /// </summary>
        [JsonPropertyName("root")]
        public string? Root { get; set; }
        /// <summary>
        /// Currently (2023-01-27) seems unused, probably intended for nesting of models in a later release
        /// </summary>
        [JsonPropertyName("parent")]
        public string? Parent { get; set; }
        /// <summary>
        /// Represents an Model with the given id/<see cref="Id"/>
        /// </summary>
        /// <param name="name">The id/<see cref="Id"/> to use.
        ///	</param>
        public Model(string name)
        {
            Id = name;
            OwnedBy = OwnedByOpenAi;
        }
        public Model() { }
        private const string OwnedByOpenAi = "openai";
    }
}
