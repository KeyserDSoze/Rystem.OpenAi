using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    /// <summary>
    /// Represents a request to the chat API.
    /// </summary>
    public sealed class ChatRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? ModelId { get; set; }
        [JsonPropertyName("messages")]
        public List<ChatMessage>? Messages { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }
        [JsonPropertyName("stream")]
        public bool Stream { get; internal set; } = false;
        [JsonPropertyName("stop")]
        public object? StopSequence { get; set; }
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty { get; set; }
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }
        [JsonPropertyName("n")]
        public int? NumberOfChoicesPerPrompt { get; set; }
        [JsonPropertyName("logit_bias")]
        public Dictionary<string, int>? Bias { get; set; }
        [JsonPropertyName("user")]
        public string? User { get; set; }
        /// <summary>
        /// This feature is in Beta. If specified, our system will make a best effort to sample deterministically,
        /// such that repeated requests with the same seed and parameters should return the same result. 
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter to monitor changes in the backend.
        /// </summary>
        [JsonPropertyName("seed")]
        public int? Seed { get; set; }
        /// <summary>
        /// Controls which (if any) function is called by the model. none means the model will not call a function and instead generates a message.
        /// auto means the model can pick between generating a message or calling a function. 
        /// Specifying a particular function via {"type: "function", "function": {"name": "my_function"}} forces the model to call that function. 
        /// none is the default when no functions are present.auto is the default if functions are present.
        /// </summary>
        [JsonPropertyName("tool_choice")]
        public object? ToolChoice { get; set; }
        /// <summary>
        /// A list of tools the model may call. Currently, only functions are supported as a tool. 
        /// Use this to provide a list of functions the model may generate JSON inputs for.
        /// </summary>
        [JsonPropertyName("tools")]
        public List<object>? Tools { get; set; }
    }
    public sealed class ToolChoice
    {
        public required string Type { get; set; }

    }
    public sealed class FunctionToolChoice
    {
        public string Name { get; set; }
    }
}
