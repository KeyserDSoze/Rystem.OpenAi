using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Message interactions, providing methods to build, manage, and execute messages within OpenAI threads.
    /// </summary>
    public interface IOpenAiMessage
    {
        /// <summary>
        /// Adds text content to the message for a specified role.
        /// </summary>
        /// <param name="role">The role of the message (e.g., user, assistant).</param>
        /// <param name="text">The text content to add.</param>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage AddText(ChatRole role, string text);

        /// <summary>
        /// Creates a builder to add complex content for a specified role.
        /// </summary>
        /// <param name="role">The role of the message (default is user).</param>
        /// <returns>A builder for creating content within the message.</returns>
        ChatMessageContentBuilder<IOpenAiMessage> AddContent(ChatRole role = ChatRole.User);

        /// <summary>
        /// Creates a builder to add content as a user.
        /// </summary>
        /// <returns>A builder for creating user-specific content within the message.</returns>
        ChatMessageContentBuilder<IOpenAiMessage> AddUserContent();

        /// <summary>
        /// Creates a builder to add content as an assistant.
        /// </summary>
        /// <returns>A builder for creating assistant-specific content within the message.</returns>
        ChatMessageContentBuilder<IOpenAiMessage> AddAssistantContent();

        /// <summary>
        /// Adds an attachment to the message with optional tools.
        /// </summary>
        /// <param name="role">The role of the message.</param>
        /// <param name="fileId">The ID of the file to attach.</param>
        /// <param name="withCodeInterpreter">Specifies if the attachment should include a code interpreter tool.</param>
        /// <param name="withFileSearch">Specifies if the attachment should include a file search tool.</param>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false);

        /// <summary>
        /// Adds metadata to the message.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage AddMetadata(string key, string value);

        /// <summary>
        /// Adds multiple metadata entries to the message.
        /// </summary>
        /// <param name="metadata">A dictionary of metadata to add.</param>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage AddMetadata(Dictionary<string, string> metadata);

        /// <summary>
        /// Clears all metadata from the message.
        /// </summary>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage ClearMetadata();

        /// <summary>
        /// Removes a specific metadata entry from the message.
        /// </summary>
        /// <param name="key">The metadata key to remove.</param>
        /// <returns>The current message instance for chaining.</returns>
        IOpenAiMessage RemoveMetadata(string key);

        /// <summary>
        /// Creates a new message in the specified thread asynchronously.
        /// </summary>
        /// <param name="threadId">The ID of the thread to add the message to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the message response.</returns>
        ValueTask<ThreadMessageResponse> CreateAsync(string threadId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a message asynchronously by its ID within a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread containing the message.</param>
        /// <param name="id">The ID of the message to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteAsync(string threadId, string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a message asynchronously by its ID within a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread containing the message.</param>
        /// <param name="id">The ID of the message to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the message response.</returns>
        ValueTask<ThreadMessageResponse> RetrieveAsync(string threadId, string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists messages asynchronously in a thread with optional filters.
        /// </summary>
        /// <param name="threadId">The ID of the thread to list messages from.</param>
        /// <param name="take">The number of messages to retrieve.</param>
        /// <param name="elementId">Optional: The ID of a reference message for pagination.</param>
        /// <param name="getAfterTheElementId">Specifies whether to retrieve messages after the reference message (true) or before (false).</param>
        /// <param name="order">The order of the messages (ascending or descending).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of message responses.</returns>
        ValueTask<ResponseAsArray<ThreadMessageResponse>> ListAsync(string threadId, int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a message asynchronously by its ID within a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread containing the message.</param>
        /// <param name="id">The ID of the message to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadMessageResponse> UpdateAsync(string threadId, string id, CancellationToken cancellationToken = default);
    }
}
