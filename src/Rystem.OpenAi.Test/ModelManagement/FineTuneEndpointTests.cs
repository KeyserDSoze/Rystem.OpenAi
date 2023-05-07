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
        //[InlineData("Azure2")]
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

            using var readableStream2 = File.OpenRead($"{location}\\Files\\validation-test-file.jsonl");
            var editableFile2 = new MemoryStream();
            await readableStream2.CopyToAsync(editableFile2);
            editableFile2.Position = 0;

            var uploadResult = await openAiApi.File
                .UploadFileAsync(editableFile, fileName);
            var uploadResult2 = await openAiApi.File
                .UploadFileAsync(editableFile2, fileName);

            await Task.Delay(15_000);

            var allFineTunes = await openAiApi.FineTune.ListAsync();
            Assert.Empty(allFineTunes.Runnings);

            var createResult = await openAiApi.FineTune.Create(uploadResult.Id)
                                    .WithModel("ada", ModelFamilyType.Ada)
                                    .WithBatchSize(1)
                                    .WithNumberOfEpochs(1)
                                    .WithLearningRateMultiplier(1)
                                    .WithValidationFile(uploadResult2.Id)
                                    .ExecuteAsync();
            Assert.NotNull(createResult);

            await Task.Delay(15_000);

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

                try
                {
                    var deleteResultForFineTune = await openAiApi.FineTune.DeleteAsync(fineTuneId);
                    Assert.NotNull(deleteResultForFineTune);
                    Assert.NotNull(deleteResultForFineTune.Id);
                }
                catch (Exception ex)
                {
                    Assert.NotNull(ex);
                }

                await Task.Delay(5_000);
                try
                {
                    var deleteResult = await openAiApi.File.DeleteAsync(uploadResult.Id);
                    if (name == "")
                        Assert.True(deleteResult.Deleted);
                }
                catch (Exception ex)
                {
                    if (name == "")
                        Assert.StartsWith("No such File object:", ex.Message);
                }
                try
                {
                    var deleteResult2 = await openAiApi.File.DeleteAsync(uploadResult2.Id);
                    if (name == "")
                        Assert.True(deleteResult2.Deleted);
                }
                catch (Exception ex)
                {
                    if (name == "")
                        Assert.StartsWith("No such File object:", ex.Message);
                }
            }
        }
    }
}
