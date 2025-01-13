using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    public interface IOpenAiUploadFile
    {
        /// <summary>
        /// The version of the API to use.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        IOpenAiUploadFile WithVersion(string version);
        /// <summary>
        /// The number of bytes in the file you are uploading.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        IOpenAiUploadFile WithSize(long bytes);
        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        IOpenAiUploadFile WithContentType(string contentType = "application/json");
        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        IOpenAiUploadFile WithContentType(MimeType mimeType);
        /// <summary>
        /// The intended purpose of the uploaded file.
        /// </summary>
        /// <param name="purpose"></param>
        /// <returns></returns>
        IOpenAiUploadFile WithPurpose(PurposeFileUpload purpose);
        /// <summary>
        /// Creates an intermediate Upload object that you can add Parts to. Currently, an Upload can accept at most 8 GB in total and expires after an hour after you create it.
        /// Once you complete the Upload, we will create a File object that contains all the parts you uploaded.This File is usable in the rest of our platform as a regular File object.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<IOpenAiPartUploadFile> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
