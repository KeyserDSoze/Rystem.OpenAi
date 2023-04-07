using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiFileApi : OpenAiBase, IOpenAiFileApi
    {
        private readonly bool _forced;
        public OpenAiFileApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations)
            : base(httpClientFactory, configurations)
        {
        }

        public async Task<List<FileResult>> AllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync<FilesData>(_configuration.GetUri(OpenAiType.File, string.Empty, _forced), _configuration, cancellationToken);
            return response.Data ?? new List<FileResult>();
        }
        private const string Purpose = "purpose";
        private const string FileContent = "file";
        public ValueTask<FileResult> UploadFileAsync(Stream file, string fileName, string purpose = "fine-tune", CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            var content = new MultipartFormDataContent
            {
                { new StringContent(purpose), Purpose },
                { new ByteArrayContent(memoryStream.ToArray()), FileContent, fileName }
            };
            return _client.PostAsync<FileResult>(_configuration.GetUri(OpenAiType.File, fileName, _forced), content, _configuration, cancellationToken);
        }
        public ValueTask<FileResult> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
            => _client.DeleteAsync<FileResult>($"{_configuration.GetUri(OpenAiType.File, fileId, _forced)}/{fileId}", _configuration, cancellationToken);
        public ValueTask<FileResult> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
            => _client.GetAsync<FileResult>($"{_configuration.GetUri(OpenAiType.File, fileId, _forced)}/{fileId}", _configuration, cancellationToken);
        public async Task<string> RetrieveFileContentAsStringAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await _client.ExecuteAsync($"{_configuration.GetUri(OpenAiType.File, fileId, _forced)}/{fileId}/content", HttpMethod.Get, null, false, _configuration, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
        private sealed class FilesData : ApiBaseResponse
        {
            [JsonPropertyName("data")]
            public List<FileResult>? Data { get; set; }
        }
    }
}
