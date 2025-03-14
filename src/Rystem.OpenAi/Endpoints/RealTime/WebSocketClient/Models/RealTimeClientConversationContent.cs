﻿using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Represents a conversation content part.
    /// </summary>
    public class RealTimeClientConversationContent
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("transcript")]
        public string? Transcript { get; set; }
    }
}
