using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadChunkMessageResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("object")]
        [JsonAnyOfChooser("thread.message.delta")]
        public string? Object { get; set; }
        [JsonPropertyName("delta")]
        public ThreadDeltaMessageResponse? Delta { get; set; }
    }
}
