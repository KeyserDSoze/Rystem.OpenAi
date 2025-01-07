using System.Collections.Generic;
using System;

namespace Rystem.OpenAi.Chat
{
    public sealed class UserChatMessage : ChatMessage
    {
        public UserChatMessage(AnyOf<string, List<ChatMessageContent>> content)
        {
            Role = ChatRole.User;
            Content = content;
        }
    }
}
