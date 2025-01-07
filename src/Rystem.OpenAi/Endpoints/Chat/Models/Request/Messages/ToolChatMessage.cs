using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class ToolChatMessage : ChatMessage
    {
        public ToolChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.Tool;
            Content = content;
        }
    }
}
