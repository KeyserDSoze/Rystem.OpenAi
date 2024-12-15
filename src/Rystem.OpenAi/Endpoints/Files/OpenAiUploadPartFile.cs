using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Files
{
    internal sealed class OpenAiUploadPartFile : IOpenAiPartUploadFile
    {
        private readonly OpenAiFile _openAiFile;
        private readonly string _uploadId;
        private readonly PartIds _parts = new();
        public OpenAiUploadPartFile(OpenAiFile openAiFile, string uploadId)
        {
            _openAiFile = openAiFile;
            _uploadId = uploadId;
        }
        private const string PartialFileContent = "data";
        public async ValueTask<FilePartResult> AddPartAsync(Stream part, CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();
            await part.CopyToAsync(memoryStream, cancellationToken);
            var fileContent = new ByteArrayContent(memoryStream.ToArray());
            var content = new MultipartFormDataContent
            {
                { fileContent, PartialFileContent }
            };
            var result = await _openAiFile.DefaultServices.HttpClientWrapper.PostAsync<FilePartResult>(_openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, string.Empty, false, $"/{_uploadId}/parts", null), content, null, _openAiFile.DefaultServices.Configuration, cancellationToken);
            _parts.Parts.Add(result.Id!);
            return result;
        }
        private static readonly object s_default = new();
        public async ValueTask<FileResult> CancelAsync(CancellationToken cancellationToken = default)
        {
            var result = await _openAiFile.DefaultServices.HttpClientWrapper.PostAsync<FileResult>(_openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, string.Empty, false, $"/{_uploadId}/cancel", null), s_default, null, _openAiFile.DefaultServices.Configuration, cancellationToken);
            return result;
        }
        public async ValueTask<FileResult> CompleteAsync(CancellationToken cancellationToken = default)
        {
            var result = await _openAiFile.DefaultServices.HttpClientWrapper.PostAsync<FileResult>(_openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, string.Empty, false, $"/{_uploadId}/complete", null), _parts, null, _openAiFile.DefaultServices.Configuration, cancellationToken);
            return result;
        }
        private sealed class PartIds
        {
            [JsonPropertyName("part_ids")]
            public List<string> Parts { get; } = [];
        }
    }
}
