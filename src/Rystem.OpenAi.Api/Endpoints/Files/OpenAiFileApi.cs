using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiFile : OpenAiBase, IOpenAiFile
    {
        private readonly bool _forced;
        public OpenAiFile(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
            _forced = false;
        }

        public async Task<List<FileResult>> AllAsync(CancellationToken cancellationToken = default)
        {
            var response = await Client.GetAsync<FilesData>(Configuration.GetUri(OpenAiType.File, string.Empty, _forced, string.Empty), Configuration, cancellationToken);
            return response.Data ?? new List<FileResult>();
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
            return Client.PostAsync<FileResult>(Configuration.GetUri(OpenAiType.File, fileName, _forced, string.Empty), content, Configuration, cancellationToken);
        }
        public ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
            => Client.DeleteAsync<FileResult>(Configuration.GetUri(OpenAiType.File, fileId, _forced, $"/{fileId}"), Configuration, cancellationToken);
        public ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
            => Client.GetAsync<FileResult>(Configuration.GetUri(OpenAiType.File, fileId, _forced, $"/{fileId}"), Configuration, cancellationToken);
        public async Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await Client.ExecuteAsync(Configuration.GetUri(OpenAiType.File, fileId, _forced, $"/{fileId}/content"), HttpMethod.Get, null, false, Configuration, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
        private sealed class FilesData : ApiBaseResponse
        {
            [JsonPropertyName("data")]
            public List<FileResult>? Data { get; set; }
        }
    }
}
