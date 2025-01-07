using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class AssistantChatMessage : ChatMessage
    {
        public AssistantChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.Assistant;
            Content = content;
        }
    }
}
