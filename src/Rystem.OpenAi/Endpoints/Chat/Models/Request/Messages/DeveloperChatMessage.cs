using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class DeveloperChatMessage : ChatMessage
    {
        public DeveloperChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.Developer;
            Content = content;
        }
    }
}
