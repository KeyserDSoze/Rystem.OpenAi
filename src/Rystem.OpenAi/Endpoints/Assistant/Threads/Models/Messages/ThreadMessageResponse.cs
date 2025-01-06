using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadMessageResponse : ThreadMessage
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        [JsonAnyOfChooser("thread.message")]
        public string? Object { get; set; }
        [JsonPropertyName("assistant_id")]
        public string? AssistantId { get; set; }
        [JsonPropertyName("thread_id")]
        public string? ThreadId { get; set; }
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
        [JsonPropertyName("run_id")]
        public string? RunId { get; set; }
    }
}
