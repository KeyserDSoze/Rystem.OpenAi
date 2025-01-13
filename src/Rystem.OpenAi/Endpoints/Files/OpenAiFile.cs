using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiFile : OpenAiBuilder<IOpenAiFile>, IOpenAiFile
    {
        public OpenAiFile(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLogger logger)
            : base(factory, configurationFactory, logger, OpenAiType.File)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.File != null)
            {
                configuration.Settings.DefaultRequestConfiguration.File.Invoke(this);
            }
        }
        public ValueTask<ResponseAsArray<FileResult>> AllAsync(CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .GetAsync<ResponseAsArray<FileResult>>(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, null, string.Empty, null),
                    null,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
        private const string Purpose = "purpose";
        private const string FileContent = "file";
        public async ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;
            return await UploadFileAsync(memoryStream.ToArray(), fileName, contentType, purpose, cancellationToken);
        }
        public ValueTask<FileResult> UploadFileAsync(byte[] file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)
        {
            var currentPurpose = purpose.ToLabel();
            using var fileContent = new ByteArrayContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            using var content = new MultipartFormDataContent
            {
                { new StringContent(currentPurpose), Purpose },
                { fileContent, FileContent, fileName }
            };
            return DefaultServices.HttpClientWrapper
                .PostAsync<FileResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, fileName, string.Empty, null),
                    content,
                    null,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
        }
        public ValueTask<FileResult> UploadFileAsync(byte[] file, string fileName, MimeType mimeType, PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)
            => UploadFileAsync(file, fileName, mimeType.Name, purpose, cancellationToken);
        public ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, MimeType mimeType, PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)
            => UploadFileAsync(file, fileName, mimeType.Name, purpose, cancellationToken);
        public ValueTask<DeleteResponse> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, fileId, $"/{fileId}", null),
                    null,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
        public ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .GetAsync<FileResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, fileId, $"/{fileId}", null),
                    null,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
        public async Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper
                .ExecuteAsync(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, fileId, $"/{fileId}/content", null),
                    HttpMethod.Get,
                    null,
                    false,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        public async Task<Stream> RetrieveFileContentAsStreamAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper
                .ExecuteAsync(
                    DefaultServices.Configuration.GetUri(OpenAiType.File, _version, fileId, $"/{fileId}/content", null),
                    HttpMethod.Get,
                    null,
                    false,
                    DefaultServices.Configuration,
                    Logger,
                    cancellationToken);
            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream, cancellationToken).NoContext();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        public IOpenAiUploadFile CreateUpload(string fileName)
            => new OpenAiUploadFile(this, fileName, Logger, _version);
    }
}
