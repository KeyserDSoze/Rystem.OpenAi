using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiFile : OpenAiBuilder<IOpenAiFile, Stream>, IOpenAiFile
    {
        public OpenAiFile(IFactory<DefaultServices> factory)
            : base(factory)
        {
        }

        public async Task<List<FileResult>> AllAsync(CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClient.GetAsync<FilesData>(DefaultServices.Configuration.GetUri(OpenAiType.File, string.Empty, Forced, string.Empty), DefaultServices.Configuration, cancellationToken);
            return response.Data ?? [];
        }
        private const string Purpose = "purpose";
        private const string FileContent = "file";
        public ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string contentType = "application/json", string purpose = "fine-tune", CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            var content = new MultipartFormDataContent
            {
                { new StringContent(purpose), Purpose },
                { fileContent, FileContent, fileName }
            };
            return DefaultServices.HttpClient.PostAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileName, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
        }
        public ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClient.DeleteAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}"), DefaultServices.Configuration, cancellationToken);
        public ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClient.GetAsync<FileResult>(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}"), DefaultServices.Configuration, cancellationToken);
        public async Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await DefaultServices.HttpClient.ExecuteAsync(DefaultServices.Configuration.GetUri(OpenAiType.File, fileId, Forced, $"/{fileId}/content"), HttpMethod.Get, null, false, DefaultServices.Configuration, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
        private sealed class FilesData : ApiBaseResponse
        {
            [JsonPropertyName("data")]
            public List<FileResult>? Data { get; set; }
        }
    }
}
