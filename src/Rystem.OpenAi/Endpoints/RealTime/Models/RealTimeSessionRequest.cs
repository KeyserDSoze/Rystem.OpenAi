using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Request for creating a real-time session.
    /// </summary>
    public class RealTimeSessionRequest : IOpenAiRequest
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        [JsonPropertyName("modalities")]
        public string[]? Modalities { get; set; }
        [JsonPropertyName("instructions")]
        public string? Instructions { get; set; }
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("input_audio_format")]
        public string? InputAudioFormat { get; set; }
        [JsonPropertyName("output_audio_format")]
        public string? OutputAudioFormat { get; set; }
        [JsonPropertyName("input_audio_transcription")]
        public RealTimeInputAudioTranscriptionRequest? InputAudioTranscription { get; set; }
        [JsonPropertyName("turn_detection")]
        public RealTimeTurnDetectionRequest? TurnDetection { get; set; }
        [JsonPropertyName("tools")]
        public List<ChatFunctionTool>? Tools { get; set; }
        [JsonPropertyName("tool_choice")]
        public AnyOf<string, ForcedFunctionTool>? ToolChoice { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("max_response_output_tokens")]
        public AnyOf<int, string>? MaxResponseOutputTokens { get; set; }
    }
}
