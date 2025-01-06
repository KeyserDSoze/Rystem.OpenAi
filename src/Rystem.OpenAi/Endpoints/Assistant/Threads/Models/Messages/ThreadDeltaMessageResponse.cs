using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class ThreadDeltaMessageResponse
    {
        [JsonPropertyName("content")]
        public AnyOf<string, List<ChatMessageContent>>? Content { get; set; }
    }
}
