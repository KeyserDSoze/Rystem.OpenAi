using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    public interface IOpenAiPartUploadFile : IOpenAiBase<IOpenAiPartUploadFile>
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
        /// <param name="index">the index from 0 (the start of the file) to N (the end of the file) to recompose the entire file.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FilePartResult> AddPartAsync(Stream part, int index, CancellationToken cancellationToken = default);
        /// <summary>
        /// Add a part to the uploaded file.
        /// </summary>
        /// <param name="part">part of the file as stream.</param>
        /// <param name="index">the index from 0 (the start of the file) to N (the end of the file) to recompose the entire file.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FilePartResult> AddPartAsync(byte[] part, int index, CancellationToken cancellationToken = default);
        /// <summary>
        /// Cancel the upload.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<FileResult> CancelAsync(CancellationToken cancellationToken = default);
    }
}
