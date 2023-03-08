using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    /// <summary>
    /// Files are used to upload documents that can be used with features like <see href="https://platform.openai.com/docs/api-reference/fine-tunes">Fine-tuning</see>.
    /// </summary>
    public interface IOpenAiFileApi
    {
        /// <summary>
        /// Returns a list of files that belong to the user's organization.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        Task<List<FileResult>> AllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns information about a specific file.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request</param>
        /// <returns></returns>
        ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns the contents of the specified file as string.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request</param>
        /// <returns></returns>
        Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete a file.
        ///	</summary>
        ///	 <param name="fileId">The ID of the file to use for this request</param>
        /// <returns></returns>
        ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="file">The stream for the file to use for this request</param>
        /// <param name="purpose">The intendend purpose of the uploaded documents. Use "fine-tune" for Fine-tuning. This allows us to validate the format of the uploaded file.</param>
        ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string purpose = "fine-tune", CancellationToken cancellationToken = default);
    }
}
