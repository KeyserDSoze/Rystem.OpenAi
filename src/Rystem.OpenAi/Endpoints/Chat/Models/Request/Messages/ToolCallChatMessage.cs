using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class ToolCallChatMessage : ChatMessage
    {
        public ToolCallChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.ToolCall;
            Content = content;
        }
    }
}
