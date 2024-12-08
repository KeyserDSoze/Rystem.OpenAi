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
        public OpenAiFile(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.File)
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
        public ValueTask<FilesDataResult> AllAsync(CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper.GetAsync<FilesDataResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, string.Empty, Forced, string.Empty), DefaultServices.Configuration, cancellationToken);
        private const string Purpose = "purpose";
        private const string FileContent = "file";
        private const string AssistantsLabel = "assistants";
        private const string AssistantsOutputLabel = "assistants_output";
        private const string BatchLabel = "batch";
        private const string BatchOutputLabel = "batch_output";
        private const string FineTuneResultsLabel = "fine-tune-results";
        private const string VisionLabel = "vision";
        private const string FineTuneLabel = "fine-tune";

        public ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string contentType = "application/json", PurposeFileUpload purpose = PurposeFileUpload.FineTune, CancellationToken cancellationToken = default)
        {
            var currentPurpose = purpose switch
            {
                PurposeFileUpload.Assistants => AssistantsLabel,
                PurposeFileUpload.AssistantsOutput => AssistantsOutputLabel,
                PurposeFileUpload.Batch => BatchLabel,
                PurposeFileUpload.BatchOutput => BatchOutputLabel,
                PurposeFileUpload.FineTuneResults => FineTuneResultsLabel,
                PurposeFileUpload.Vision => VisionLabel,
                _ => FineTuneLabel
            };

            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            var content = new MultipartFormDataContent
            {
                { new StringContent(currentPurpose), Purpose },
                { fileContent, FileContent, fileName }
            };
            return DefaultServices.HttpClientWrapper.PostAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileName, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
        }
        public ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper.DeleteAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}"), DefaultServices.Configuration, cancellationToken);
        public ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper.GetAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}"), DefaultServices.Configuration, cancellationToken);
        public async Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper.ExecuteAsync(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}/content"), HttpMethod.Get, null, false, DefaultServices.Configuration, cancellationToken);
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        public async Task<Stream> RetrieveFileContentAsStreamAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClientWrapper.ExecuteAsync(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}/content"), HttpMethod.Get, null, false, DefaultServices.Configuration, cancellationToken);
            var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream, cancellationToken).NoContext();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
