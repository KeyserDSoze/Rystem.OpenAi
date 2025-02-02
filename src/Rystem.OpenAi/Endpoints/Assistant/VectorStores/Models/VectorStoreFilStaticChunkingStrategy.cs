﻿using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public class VectorStoreFilStaticChunkingStrategy
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("static")]
        public VectorStoreFilStaticChunkingDetails? StaticDetails { get; set; }
    }
}
