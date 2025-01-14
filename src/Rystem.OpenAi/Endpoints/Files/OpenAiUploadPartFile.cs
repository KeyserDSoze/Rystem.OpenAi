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
        private readonly IOpenAiLoggerFactory _loggerFactory;
        private readonly PartIds _parts = new();
        private string? _version;
        public OpenAiUploadPartFile(OpenAiFile openAiFile, string uploadId, IOpenAiLoggerFactory loggerFactory, string? version)
        {
            _openAiFile = openAiFile;
            _uploadId = uploadId;
            _loggerFactory = loggerFactory;
            _version = version;
        }
        private const string PartialFileContent = "data";
        public IOpenAiPartUploadFile WithVersion(string version)
        {
            _version = version;
            return this;
        }
        public async ValueTask<FilePartResult> AddPartAsync(Stream part, CancellationToken cancellationToken = default)
        {
            if (part.CanSeek)
                part.Seek(0, SeekOrigin.Begin);
            using var fileContent = new ByteArrayContent(await part.ToArrayAsync());
            using var content = new MultipartFormDataContent
            {
                { fileContent, PartialFileContent }
            };

            var result = await _openAiFile.DefaultServices.HttpClientWrapper
                .PostAsync<FilePartResult>(
                    _openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, _version, null, $"/{_uploadId}/parts", null),
                    content,
                    null,
                    _openAiFile.DefaultServices.Configuration,
                    _loggerFactory.Create(),
                    cancellationToken);
            _parts.Parts.Add(result.Id!);
            return result;
        }
        private static readonly object s_default = new();
        public async ValueTask<FileResult> CancelAsync(CancellationToken cancellationToken = default)
        {
            var result = await _openAiFile.DefaultServices.HttpClientWrapper
                .PostAsync<FileResult>(
                    _openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, _version, null, $"/{_uploadId}/cancel", null),
                    s_default,
                    null,
                    _openAiFile.DefaultServices.Configuration,
                    _loggerFactory.Create(),
                    cancellationToken);
            return result;
        }
        public async ValueTask<FileResult> CompleteAsync(CancellationToken cancellationToken = default)
        {
            var result = await _openAiFile.DefaultServices.HttpClientWrapper.
                PostAsync<FileResult>(
                    _openAiFile.DefaultServices.Configuration.GetUri(OpenAiType.Upload, _version, null, $"/{_uploadId}/complete", null),
                    _parts,
                    null,
                    _openAiFile.DefaultServices.Configuration,
                    _loggerFactory.Create(),
                    cancellationToken);
            return result;
        }
        private sealed class PartIds
        {
            [JsonPropertyName("part_ids")]
            public List<string> Parts { get; } = [];
        }
    }
}
