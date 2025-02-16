using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Response returned by the create session endpoint.
    /// </summary>
    public class RealTimeSessionResponse : ApiBaseResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
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
        public string? ToolChoice { get; set; }
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }
        [JsonPropertyName("max_response_output_tokens")]
        public AnyOf<int, string>? MaxResponseOutputTokens { get; set; }
        [JsonPropertyName("client_secret")]
        public ClientSecret? ClientSecret { get; set; }
    }
}
