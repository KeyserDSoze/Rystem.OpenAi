using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class FineTuneEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public FineTuneEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask AllFlowAsync()
        {
            Assert.NotNull(_openAiApi.FineTune);
            var name = "data-test-file.jsonl";
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\data-test-file.jsonl");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var uploadResult = await _openAiApi.File
                .UploadFileAsync(editableFile, name);

            var allFineTunes = await _openAiApi.FineTune.ListAsync();
            Assert.Empty(allFineTunes.Data.Where(x => x.Status != "cancelled"));

            var createResult = await _openAiApi.FineTune.Create(uploadResult.Id)
                                    .ExecuteAsync();
            Assert.NotNull(createResult);

            allFineTunes = await _openAiApi.FineTune.ListAsync();
            Assert.NotEmpty(allFineTunes.Data);

            var fineTuneId = allFineTunes.Data.First().Id;
            var retrieveFineTune = await _openAiApi.FineTune.RetrieveAsync(fineTuneId);
            Assert.NotNull(retrieveFineTune);

            var events = await _openAiApi.FineTune.ListEventsAsync(fineTuneId);
            Assert.NotNull(events);

            var cancelResult = await _openAiApi.FineTune.CancelAsync(fineTuneId);
            Assert.NotNull(cancelResult);

            var deleteResult = await _openAiApi.File.DeleteAsync(uploadResult.Id);
            Assert.True(deleteResult.Deleted);
        }

    }
}
