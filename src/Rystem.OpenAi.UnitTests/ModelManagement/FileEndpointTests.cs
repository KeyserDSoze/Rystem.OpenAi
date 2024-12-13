using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Files;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class FileEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        private readonly IOpenAiUtility _openAiUtility;

        public FileEndpointTests(IFactory<IOpenAi> openAiFactory, IOpenAiUtility openAiUtility)
        {
            _openAiFactory = openAiFactory;
            _openAiUtility = openAiUtility;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask AllFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
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
            foreach (var result in results?.Data!)
                await openAiApi.File.DeleteAsync(result.Id!);
            results = await openAiApi.File
               .AllAsync();

            Assert.Empty(results?.Data!);

            var uploadResult = await openAiApi.File
                .UploadFileAsync(editableFile, fileName);

            Assert.True(uploadResult.Id?.Length > 10);
            Assert.Contains("file", uploadResult.Id);

            results = await openAiApi.File
                .AllAsync();
            Assert.NotEmpty(results.Data ?? []);


            var retrieve = await openAiApi.File.RetrieveAsync(uploadResult.Id);
            Assert.NotNull(retrieve);
            Assert.Equal("data-test-file.jsonl", retrieve.Name);

#pragma warning disable S125 // Not allowed with free apikey
            //var contentRetrieve = await _openAiApi.File.RetrieveFileContentAsStringAsync(uploadResult.Id);
            //Assert.Contains("type for", contentRetrieve);
#pragma warning restore S125 // Sections of code should not be commented out
            await Task.Delay(5_000);
            var deleteResult = await openAiApi.File.DeleteAsync(uploadResult.Id);
            if (name != "Azure")
                Assert.True(deleteResult.Deleted);

            await Task.Delay(5_000);
            results = await openAiApi.File.AllAsync();
            Assert.Empty(results?.Data!);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask PartialUploadAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
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
            foreach (var result in results?.Data!)
                await openAiApi.File.DeleteAsync(result.Id!);
            results = await openAiApi.File
               .AllAsync();

            Assert.Empty(results?.Data!);

            var upload = openAiApi.File
                .CreateUpload(fileName)
                .WithPurpose(PurposeFileUpload.FineTune)
                .WithContentType("application/json")
                .WithSize(editableFile.Length);

            var execution = await upload.ExecuteAsync();
            var partResult = await execution.AddPartAsync(editableFile);
            Assert.True(partResult.Id?.Length > 7);
            var completeResult = await execution.CompleteAsync();

            Assert.True(completeResult.Id?.Length > 10);
            Assert.Contains("file", completeResult.Id);
        }
    }
}
