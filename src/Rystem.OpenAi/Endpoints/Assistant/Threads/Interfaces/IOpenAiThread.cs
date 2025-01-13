using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Thread interactions, providing methods to build, manage, and execute threads with OpenAI services.
    /// </summary>
    public interface IOpenAiThread : IOpenAiBase<IOpenAiThread>
    {
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
        /// Add a message to the request as User
        /// </summary>
        /// <returns>Builder</returns>
        IMessageThreadBuilder<IOpenAiThread> WithMessage();
        /// <summary>
        /// Sets the ID of the thread.
        /// </summary>
        /// <param name="id">Thread Id</param>
        /// <returns></returns>
        IOpenAiThread WithId(string id);
        /// <summary>
        /// Creates a new thread asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a thread asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a thread asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> RetrieveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a thread asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the thread response.</returns>
        ValueTask<ThreadResponse> UpdateAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Asynchronously adds messages to a thread and yields the responses.
        /// </summary>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>An asynchronous enumerable of <see cref="ThreadMessageResponse"/>.</returns>
        IAsyncEnumerable<ThreadMessageResponse> AddMessagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously deletes a specific message by its ID.
        /// </summary>
        /// <param name="id">The ID of the message to delete.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>A <see cref="DeleteResponse"/> containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteMessageAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves a specific message by its ID.
        /// </summary>
        /// <param name="id">The ID of the message to retrieve.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>A <see cref="ValueTask"/> containing the retrieved message response.</returns>
        ValueTask<ThreadMessageResponse> RetrieveMessageAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously lists messages in a thread with optional filters and ordering.
        /// </summary>
        /// <param name="take">The maximum number of messages to retrieve.</param>
        /// <param name="elementId">The ID of a specific element to filter results around.</param>
        /// <param name="getAfterTheElementId">If true, retrieves messages after the specified element ID; otherwise, retrieves messages before.</param>
        /// <param name="order">The order in which to retrieve messages (ascending or descending).</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>A <see cref="ValueTask"/> containing an array of message responses.</returns>
        ValueTask<ResponseAsArray<ThreadMessageResponse>> ListMessagesAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously updates a specific message by its ID.
        /// </summary>
        /// <param name="id">The ID of the message to update.</param>
        /// <param name="cancellationToken">Token to observe for cancellation requests.</param>
        /// <returns>A <see cref="ValueTask"/> containing the updated message response.</returns>
        ValueTask<ThreadMessageResponse> UpdateMessageAsync(string id, CancellationToken cancellationToken = default);
    }
}
