using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class FileEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public FileEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask AllFlowAsync()
        {
            Assert.NotNull(_openAiApi.File);
            var name = "data-test-file.jsonl";
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\data-test-file.jsonl");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await _openAiApi.File
                .AllAsync();
            foreach (var result in results)
                await _openAiApi.File.DeleteAsync(result.Id);
            results = await _openAiApi.File
               .AllAsync();

            Assert.Empty(results);

            var uploadResult = await _openAiApi.File
                .UploadFileAsync(editableFile, name);

            Assert.True(uploadResult.Id.Length > 10);
            Assert.Contains("file", uploadResult.Id);

            results = await _openAiApi.File
                .AllAsync();
            Assert.NotEmpty(results);


            var retrieve = await _openAiApi.File.RetrieveAsync(uploadResult.Id);
            Assert.NotNull(retrieve);
            Assert.Equal("data-test-file.jsonl", retrieve.Name);

#pragma warning disable S125 // Not allowed with free apikey
            //var contentRetrieve = await _openAiApi.File.RetrieveFileContentAsStringAsync(uploadResult.Id);
            //Assert.Contains("type for", contentRetrieve);
#pragma warning restore S125 // Sections of code should not be commented out

            var deleteResult = await _openAiApi.File.DeleteAsync(uploadResult.Id);
            Assert.True(deleteResult.Deleted);

            results = await _openAiApi.File
                .AllAsync();
            Assert.Empty(results);
        }

    }
}
