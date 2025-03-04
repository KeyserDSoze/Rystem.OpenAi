using System.Text.Json.Serialization;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Client event: input_audio_buffer.append.
    /// </summary>
    public class RealTimeClientInputAudioBufferAppendEvent : RealTimeClientEvent
    {
        [JsonPropertyName("audio")]
        public string? Audio { get; set; }
    }
}
