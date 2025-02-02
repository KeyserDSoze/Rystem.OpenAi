﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public class AssistantRequest : UnixTimeBase, IOpenAiRequest
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonIgnore]
        public StringBuilder? InstructionsBuilder { get; set; }
        [JsonPropertyName("instructions")]
        public string? Instructions { get => InstructionsBuilder?.ToString(); set => InstructionsBuilder = new(value); }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
        [JsonPropertyName("response_format")]
        public AnyOf<string, ResponseFormatChatRequest>? ResponseFormat { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("tools")]
        public List<AnyOf<AssistantCodeInterpreterTool, AssistantFileSearchTool, AssistantFunctionTool>>? Tools { get; set; }
        [JsonPropertyName("tool_resources")]
        public AssistantToolResources? ToolResources { get; set; }
    }
}
