using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiUploadFile : IOpenAiUploadFile
    {
        private readonly OpenAiFile _openAiFile;
        private readonly IOpenAiLogger _logger;
        private readonly FilePartialStartRequest _request;
        public OpenAiUploadFile(OpenAiFile openAiFile, string fileName, IOpenAiLogger logger)
        {
            _openAiFile = openAiFile;
            _logger = logger;
            _request = new()
            {
                FileName = fileName
            };
        }

        public async ValueTask<IOpenAiPartUploadFile> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var response = await _openAiFile.DefaultServices.HttpClientWrapper
                .PostAsync<FileResult>(
                    _openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, string.Empty, false, string.Empty, null),
                    _request,
                    null,
                    _openAiFile.DefaultServices.Configuration,
                    _logger,
                    cancellationToken);
            return new OpenAiUploadPartFile(_openAiFile, response.Id!, _logger);
        }
        public IOpenAiUploadFile WithContentType(string contentType = "application/json")
        {
            _request.MimeType = contentType;
            return this;
        }
        public IOpenAiUploadFile WithContentType(MimeType mimeType)
            => WithContentType(mimeType.Name);
        public IOpenAiUploadFile WithPurpose(PurposeFileUpload purpose)
        {
            _request.Purpose = purpose.ToLabel();
            return this;
        }
        public IOpenAiUploadFile WithSize(long numberOfBytes)
        {
            _request.Bytes = numberOfBytes;
            return this;
        }
    }
}
