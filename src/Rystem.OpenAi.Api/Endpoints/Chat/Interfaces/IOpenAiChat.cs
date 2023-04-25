using System;

namespace Rystem.OpenAi.Chat
{
    public interface IOpenAiChat
    {
        /// <summary>
        /// Given a chat conversation, the model will return a chat completion response.
        /// </summary>
        /// <param name="message">The messages to generate chat completions for, in the chat format.</param>
        /// <returns>Builder</returns>
        ChatRequestBuilder Request(ChatMessage message);
    }
}
