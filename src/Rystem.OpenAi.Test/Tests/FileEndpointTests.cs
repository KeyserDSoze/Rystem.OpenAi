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
        private readonly IOpenAiFactory _openAiFactory;
        public FileEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask AllFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.File);
            var fileName = "data-test-file.jsonl";
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\data-test-file.jsonl");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var results = await openAiApi.File
                .AllAsync();
            foreach (var result in results)
                await openAiApi.File.DeleteAsync(result.Id);
            results = await openAiApi.File
               .AllAsync();

            Assert.Empty(results);

            var uploadResult = await openAiApi.File
                .UploadFileAsync(editableFile, fileName);

            Assert.True(uploadResult.Id.Length > 10);
            Assert.Contains("file", uploadResult.Id);

            results = await openAiApi.File
                .AllAsync();
            Assert.NotEmpty(results);


            var retrieve = await openAiApi.File.RetrieveAsync(uploadResult.Id);
            Assert.NotNull(retrieve);
            Assert.Equal("data-test-file.jsonl", retrieve.Name);

#pragma warning disable S125 // Not allowed with free apikey
            //var contentRetrieve = await _openAiApi.File.RetrieveFileContentAsStringAsync(uploadResult.Id);
            //Assert.Contains("type for", contentRetrieve);
#pragma warning restore S125 // Sections of code should not be commented out
            await Task.Delay(5_000);
            var deleteResult = await openAiApi.File.DeleteAsync(uploadResult.Id);
            Assert.True(deleteResult.Deleted);

            results = await openAiApi.File
                .AllAsync();
            Assert.Empty(results);
        }

    }
}
