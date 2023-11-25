using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Chat
{
    public sealed class StreamingChatResult
    {
        [JsonPropertyName("chunks")]
        public List<ChatResult> Chunks { get; set; } = null!;
        [JsonPropertyName("lastChunk")]
        public ChatResult LastChunk => Chunks.LastOrDefault();
    }
}
