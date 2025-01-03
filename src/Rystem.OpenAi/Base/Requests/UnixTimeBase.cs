using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public abstract class UnixTimeBase
    {
        /// The time when the result was generated
        [JsonIgnore]
        public DateTime? Created
        {
            get => CreatedAt.HasValue ? DateTimeOffset.FromUnixTimeSeconds(CreatedAt.Value).DateTime : null;
            set => CreatedAt = value.HasValue ? new DateTimeOffset(value.Value).ToUnixTimeSeconds() : null;
        }
        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created_at")]
        public long? CreatedAt { get; set; }
    }
}
