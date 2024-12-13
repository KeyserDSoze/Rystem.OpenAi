using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    public interface IOpenAiUploadFile
    {
        /// <summary>
        /// Complete the upload.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FileResult> CompleteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Add a part to the uploaded file.
        /// </summary>
        /// <param name="part">part of the file as stream.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FilePartResult> AddPartAsync(Stream part, CancellationToken cancellationToken = default);
        /// <summary>
        /// Cancel the upload.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FileResult> CancelAsync(CancellationToken cancellationToken = default);
    }
}
