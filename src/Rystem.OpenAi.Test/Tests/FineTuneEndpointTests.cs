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
        private readonly IOpenAiFactory _openAiFactory;
        public FineTuneEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        public async ValueTask AllFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.FineTune);
            var fileName = "data-test-file.jsonl";
            var location = Assembly.GetExecutingAssembly().Location;
            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
            using var readableStream = File.OpenRead($"{location}\\Files\\data-test-file.jsonl");
            var editableFile = new MemoryStream();
            await readableStream.CopyToAsync(editableFile);
            editableFile.Position = 0;

            var uploadResult = await openAiApi.File
                .UploadFileAsync(editableFile, fileName);

            var allFineTunes = await openAiApi.FineTune.ListAsync();
            Assert.Empty(allFineTunes.Runnings);

            var createResult = await openAiApi.FineTune.Create(uploadResult.Id)
                                    .ExecuteAsync();
            Assert.NotNull(createResult);

            allFineTunes = await openAiApi.FineTune.ListAsync();
            var inExecutionOrExecuted = allFineTunes.Runnings.ToList();
            inExecutionOrExecuted.AddRange(allFineTunes.Succeeded);
            inExecutionOrExecuted.AddRange(allFineTunes.Pendings);
            Assert.NotEmpty(inExecutionOrExecuted);

            foreach (var fineTune in inExecutionOrExecuted)
            {
                var fineTuneId = fineTune.Id;
                var retrieveFineTune = await openAiApi.FineTune.RetrieveAsync(fineTuneId);
                Assert.NotNull(retrieveFineTune);

                var events = await openAiApi.FineTune.ListEventsAsync(fineTuneId);
                Assert.NotNull(events);

                var cancelResult = await openAiApi.FineTune.CancelAsync(fineTuneId);
                Assert.NotNull(cancelResult);

                await Task.Delay(5_000);
                try
                {
                    var deleteResult = await openAiApi.File.DeleteAsync(uploadResult.Id);
                    Assert.True(deleteResult.Deleted);
                }
                catch (Exception ex)
                {
                    Assert.StartsWith("No such File object:", ex.Message);
                }
            }
        }
    }
}
