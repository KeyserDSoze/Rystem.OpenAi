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
        /// <summary>
        /// Start chat builder with a user message. User message is a message used to send information.
        /// </summary>
        /// <param name="message">The messages to generate chat completions for, in the chat format.</param>
        /// <returns>Builder</returns>
        ChatRequestBuilder RequestWithUserMessage(string message);
        /// <summary>
        /// Start chat builder with a system message. System message is a message used to improve the response from chat api.
        /// </summary>
        /// <param name="message">The messages to generate chat completions for, in the chat format.</param>
        /// <returns>Builder</returns>
        ChatRequestBuilder RequestWithSystemMessage(string message);
        /// <summary>
        /// Start chat builder with an assistant message. Assistant message is the response from chat api, usually you don't need to set this message.
        /// </summary>
        /// <param name="message">The messages to generate chat completions for, in the chat format.</param>
        /// <returns>Builder</returns>
        ChatRequestBuilder RequestWithAssistantMessage(string message);
    }
}
