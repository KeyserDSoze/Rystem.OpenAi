using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Represents the configuration for response.create.
    /// </summary>
    public class RealTimeClientResponseCreateData
    {
        [JsonPropertyName("modalities")]
        public List<string> Modalities { get; set; }

        [JsonPropertyName("instructions")]
        public string Instructions { get; set; }

        [JsonPropertyName("voice")]
        public string Voice { get; set; }

        [JsonPropertyName("output_audio_format")]
        public string OutputAudioFormat { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        // Additional properties (tools, tool_choice, max_output_tokens, etc.) can be added.
    }
}
