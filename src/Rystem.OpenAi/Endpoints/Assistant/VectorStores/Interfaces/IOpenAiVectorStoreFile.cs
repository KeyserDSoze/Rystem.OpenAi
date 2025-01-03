using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Vector Store File interactions, providing methods to manage files and file batches in a vector store.
    /// </summary>
    public interface IOpenAiVectorStoreFile
    {
        /// <summary>
        /// Configures the vector store file with a single file ID.
        /// </summary>
        /// <param name="fileId">The ID of the file to associate with the vector store.</param>
        /// <returns>The current vector store file instance for chaining.</returns>
        IOpenAiVectorStoreFile WithFile(string fileId);

        /// <summary>
        /// Configures the vector store file with multiple file IDs.
        /// </summary>
        /// <param name="fileIds">A collection of file IDs to associate with the vector store.</param>
        /// <returns>The current vector store file instance for chaining.</returns>
        IOpenAiVectorStoreFile WithFiles(IEnumerable<string> fileIds);

        /// <summary>
        /// Creates a vector store file asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the created vector store file.</returns>
        ValueTask<VectorStoreFile> CreateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a vector store file asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the file to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the delete response.</returns>
        ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists vector store files asynchronously with optional filters.
        /// </summary>
        /// <param name="take">The number of files to retrieve.</param>
        /// <param name="elementId">Optional: The ID of a reference file for pagination.</param>
        /// <param name="getAfterTheElementId">Specifies whether to retrieve files after the reference file (true) or before (false).</param>
        /// <param name="order">The order of the files (ascending or descending).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of vector store files.</returns>
        ValueTask<ResponseAsArray<VectorStoreFile>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a vector store file asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the file to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved vector store file.</returns>
        ValueTask<VectorStoreFile> RetrieveAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a batch of vector store files asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the created batch of vector store files.</returns>
        ValueTask<VectorStoreFileBatch> CreateBatchAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels a batch of vector store files asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the batch to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the canceled batch of vector store files.</returns>
        ValueTask<VectorStoreFileBatch> CancelBatchAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists files in a batch asynchronously with optional filters.
        /// </summary>
        /// <param name="batchId">The ID of the batch to list files from.</param>
        /// <param name="take">The number of files to retrieve.</param>
        /// <param name="elementId">Optional: The ID of a reference file for pagination.</param>
        /// <param name="getAfterTheElementId">Specifies whether to retrieve files after the reference file (true) or before (false).</param>
        /// <param name="order">The order of the files (ascending or descending).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of files in the batch.</returns>
        ValueTask<ResponseAsArray<VectorStoreFile>> ListFilesInBatchAsync(string batchId, int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a batch of vector store files asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the batch to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved batch of vector store files.</returns>
        ValueTask<VectorStoreFileBatch> RetrieveBatchAsync(string id, CancellationToken cancellationToken = default);
    }
}
