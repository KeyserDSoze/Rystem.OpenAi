using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class SystemChatMessage : ChatMessage
    {
        public SystemChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.System;
            Content = content;
        }
    }
}
