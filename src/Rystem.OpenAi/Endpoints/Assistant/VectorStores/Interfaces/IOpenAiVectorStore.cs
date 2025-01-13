using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Vector Store interactions, providing methods to create, manage, and retrieve vector stores and their associated files.
    /// </summary>
    public interface IOpenAiVectorStore : IOpenAiWithMetadata<IOpenAiVectorStore>, IOpenAiBase<IOpenAiVectorStore>
    {
        /// <summary>
        /// Manages files within a specific vector store.
        /// </summary>
        /// <returns>An interface for managing vector store files.</returns>
        IOpenAiVectorStoreFile ManageStore();

        /// <summary>
        /// Sets the name of the vector store.
        /// </summary>
        /// <param name="name">The name of the vector store.</param>
        /// <returns>The current vector store instance for chaining.</returns>
        IOpenAiVectorStore WithName(string name);

        /// <summary>
        /// Adds a single file to the vector store.
        /// </summary>
        /// <param name="fileId">The ID of the file to add.</param>
        /// <returns>The current vector store instance for chaining.</returns>
        IOpenAiVectorStore AddFile(string fileId);

        /// <summary>
        /// Adds multiple files to the vector store.
        /// </summary>
        /// <param name="fileIds">A collection of file IDs to add.</param>
        /// <returns>The current vector store instance for chaining.</returns>
        IOpenAiVectorStore AddFiles(IEnumerable<string> fileIds);

        /// <summary>
        /// Configures the vector store to expire after a specific number of days.
        /// </summary>
        /// <param name="days">The number of days before expiration.</param>
        /// <returns>The current vector store instance for chaining.</returns>
        IOpenAiVectorStore WithExpirationAfter(int days);
        /// <summary>
        /// Sets the ID of the vector store.
        /// </summary>
        /// <param name="id">Vector Id</param>
        /// <returns>The current vector store instance for chaining.</returns>
        IOpenAiVectorStore WithId(string id);

        /// <summary>
        /// Creates a new vector store asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the created vector store result.</returns>
        ValueTask<VectorStoreResult> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a vector store asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists vector stores asynchronously with optional filters.
        /// </summary>
        /// <param name="take">The number of vector stores to retrieve.</param>
        /// <param name="elementId">Optional: The ID of a reference vector store for pagination.</param>
        /// <param name="getAfterTheElementId">Specifies whether to retrieve vector stores after the reference vector store (true) or before (false).</param>
        /// <param name="order">The order of the vector stores (ascending or descending).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of vector store results.</returns>
        ValueTask<ResponseAsArray<VectorStoreResult>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a vector store asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved vector store result.</returns>
        ValueTask<VectorStoreResult> RetrieveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a vector store asynchronously by its ID.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated vector store result.</returns>
        ValueTask<VectorStoreResult> UpdateAsync(CancellationToken cancellationToken = default);
    }
}
