using System.Collections.Generic;

namespace Rystem.OpenAi.Assistant
{
    public interface IOpenAiFileSearchToolResourcesAssistant<T>
    {
        /// <summary>
        /// The list of files to search over. Each file must be less than 100MB in size. The total size of all files combined must be less than 1GB. The supported file formats are: PDF, DOC, DOCX, PPT, PPTX, XLSX, and TXT.
        /// </summary>
        /// <param name="filesId"></param>
        /// <returns></returns>
        T Use(params string[] filesId);
        /// <summary>
        /// The chunking strategy used to chunk the file(s). 
        /// </summary>
        /// <param name="maxChunkSize">The maximum number of tokens in each chunk. The default value is 800. The minimum value is 100 and the maximum value is 4096.</param>
        /// <param name="chunkOverlap">The number of tokens that overlap between chunks. The default value is 400.
        /// Note that the overlap must not exceed half of max_chunk_size_tokens.
        /// </param>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> WithStaticChunkingStrategy(int maxChunkSize, int chunkOverlap);
        /// <summary>
        /// Set of 16 key-value pairs that can be attached to a vector store. This can be useful for storing additional information about the vector store in a structured format. Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(string key, string value);
        /// <summary>
        /// Set of 16 key-value pairs that can be attached to a vector store. This can be useful for storing additional information about the vector store in a structured format. Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> AddMetadata(Dictionary<string, string> metadata);
        /// <summary>
        /// Remove a metadata key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> RemoveMetadata(string key);
        /// <summary>
        /// Clear all metadata.
        /// </summary>
        /// <returns></returns>
        IOpenAiFileSearchToolResourcesAssistant<T> ClearMetadata();
    }
}
