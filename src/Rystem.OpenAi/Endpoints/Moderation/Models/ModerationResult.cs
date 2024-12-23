﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationResult
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("results")]
        public List<ModerationData>? Results { get; set; }
    }
}
