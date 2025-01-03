using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Thread interactions, providing methods to build, manage, and execute threads with OpenAI services.
    /// </summary>
    public interface IOpenAiThread
    {
        /// <summary>
        /// Adds a message to the thread for a specified role, with optional tools.
        /// </summary>
        /// <param name="role"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        IOpenAiThread AddText(ChatRole role, string text);
        /// <summary>
        /// Adds an attachment to the thread for a specified role, with optional tools.
        /// </summary>
        /// <param name="role">The role of the message.</param>
        /// <param name="fileId">The ID of the file to attach.</param>
        /// <param name="withCodeInterpreter">Specifies if the attachment should include a code interpreter tool.</param>
        /// <param name="withFileSearch">Specifies if the attachment should include a file search tool.</param>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false);
        /// <summary>
        /// Adds metadata to the thread for a specified role.
        /// </summary>
        /// <param name="role">The role of the message.</param>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread AddMetadata(ChatRole role, string key, string value);

        /// <summary>
        /// Adds metadata to the entire thread.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread AddMetadata(string key, string value);

        /// <summary>
        /// Adds multiple metadata entries to the thread.
        /// </summary>
        /// <param name="metadata">A dictionary of metadata to add.</param>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread AddMetadata(Dictionary<string, string> metadata);

        /// <summary>
        /// Clears all metadata from the thread.
        /// </summary>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread ClearMetadata();

        /// <summary>
        /// Removes a specific metadata entry from the thread.
        /// </summary>
        /// <param name="key">The metadata key to remove.</param>
        /// <returns>The current thread instance for chaining.</returns>
        IOpenAiThread RemoveMetadata(string key);

        /// <summary>
        /// Provides access to tool resources for the thread.
        /// </summary>
        /// <returns>An assistant interface for tool resources.</returns>
        IOpenAiToolResourcesAssistant<IOpenAiThread> WithToolResources();
        /// <summary>
        /// Add a message with content text or image or audio to the request as User
        /// </summary>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder<IOpenAiThread> AddUserContent();
        /// <summary>
        /// Add a message with content text or image or audio to the request as Assistant
        /// </summary>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder<IOpenAiThread> AddAssistantContent();
        /// <summary>
        /// Creates a new thread asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a thread asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the thread to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a thread asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the thread to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> RetrieveAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a thread asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the thread to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> UpdateAsync(string id, CancellationToken cancellationToken = default);
    }
}
