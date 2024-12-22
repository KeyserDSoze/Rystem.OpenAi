using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    /// <summary>
    /// Files are used to upload documents that can be used with features like <see href="https://platform.openai.com/docs/api-reference/fine-tunes">Fine-tuning</see>.
    /// </summary>
    public interface IOpenAiFile
    {
        /// <summary>
        /// Returns a list of files that belong to the user's organization.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        ValueTask<FilesDataResult> AllAsync(CancellationToken cancellationToken = default);
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
        /// Returns the contents of the specified file as stream.
        /// </summary>
        /// <param name="fileId">The ID of the file to use for this request</param>
        /// <returns></returns>
        Task<Stream> RetrieveFileContentAsStreamAsync(string fileId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete a file.
        ///	</summary>
        ///	 <param name="fileId">The ID of the file to use for this request</param>
        /// <returns></returns>
        ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="file">The stream for the file to use for this request.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="contentType">The type of the file.</param>
        /// <param name="purpose">The intendend purpose of the uploaded documents. Use "fine-tune" for Fine-tuning. This allows us to validate the format of the uploaded file.</param>
        ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default);
        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="file">The stream for the file to use for this request.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="mimeType">The type of the file.</param>
        /// <param name="purpose">The intendend purpose of the uploaded documents. Use "fine-tune" for Fine-tuning. This allows us to validate the format of the uploaded file.</param>
        ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, MimeType mimeType, PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default);
        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="file">The byte array for the file to use for this request.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="contentType">The type of the file.</param>
        /// <param name="purpose">The intendend purpose of the uploaded documents. Use "fine-tune" for Fine-tuning. This allows us to validate the format of the uploaded file.</param>
        ValueTask<FileResult> UploadFileAsync(byte[] file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default);
        /// <summary>
        /// Upload a file that contains document(s) to be used across various endpoints/features. Currently, the size of all the files uploaded by one organization can be up to 1 GB. Please contact us if you need to increase the storage limit.
        /// </summary>
        /// <param name="file">The byte array for the file to use for this request.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="mimeType">The type of the file.</param>
        /// <param name="purpose">The intendend purpose of the uploaded documents. Use "fine-tune" for Fine-tuning. This allows us to validate the format of the uploaded file.</param>
        ValueTask<FileResult> UploadFileAsync(byte[] file, string fileName, MimeType mimeType, PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates an intermediate Upload object that you can add Parts to. Currently, an Upload can accept at most 8 GB in total and expires after an hour after you create it.
        /// Once you complete the Upload, we will create a File object that contains all the parts you uploaded.This File is usable in the rest of our platform as a regular File object.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IOpenAiUploadFile CreateUpload(string fileName);
    }
}
