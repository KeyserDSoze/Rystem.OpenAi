﻿//using System.Reflection;
//using Microsoft.Extensions.DependencyInjection;
//using Rystem.OpenAi.FineTune;
//using Xunit;

//namespace Rystem.OpenAi.Test
//{
//    public class FineTuneEndpointTests
//    {
//        private readonly IFactory<IOpenAi> _openAiFactory;
//        private readonly IOpenAiUtility _openAiUtility;

//        public FineTuneEndpointTests(IFactory<IOpenAi> openAiFactory, IOpenAiUtility openAiUtility)
//        {
//            _openAiFactory = openAiFactory;
//            _openAiUtility = openAiUtility;
//        }
//        [Theory]
//        [InlineData("")]
//        [InlineData("Azure2")]
//        public async ValueTask AllFlowAsync(string name)
//        {
//            var openAiApi = _openAiFactory.Create(name);
//            Assert.NotNull(openAiApi.FineTune);
//            var fileName = "data-test-file.jsonl";
//            var location = Assembly.GetExecutingAssembly().Location;
//            location = string.Join('\\', location.Split('\\').Take(location.Split('\\').Length - 1));
//            using var readableStream = File.OpenRead($"{location}\\Files\\data-test-file.jsonl");
//            var editableFile = new MemoryStream();
//            await readableStream.CopyToAsync(editableFile);
//            editableFile.Position = 0;

//            using var readableStream2 = File.OpenRead($"{location}\\Files\\validation-test-file.jsonl");
//            var editableFile2 = new MemoryStream();
//            await readableStream2.CopyToAsync(editableFile2);
//            editableFile2.Position = 0;

//            var uploadResult = await openAiApi.File
//                .UploadFileAsync(editableFile, fileName);
//            var uploadResult2 = await openAiApi.File
//                .UploadFileAsync(editableFile2, fileName);

//            await Task.Delay(15_000);

//            var allFineTunes = await openAiApi.FineTune.ListAsync();
//            Assert.Empty(allFineTunes.Runnings);

//            var createResult = await openAiApi.FineTune
//                                    .WithModel(FineTuningModelName.Gpt_4o_2024_08_06)
//                                    .WithValidationFile(uploadResult2.Id)
//                                    .WithFileId(uploadResult.Id)
//                                    .ExecuteAsync();
//            Assert.NotNull(createResult);

//            await Task.Delay(15_000);

//            allFineTunes = await openAiApi.FineTune.ListAsync();
//            var inExecutionOrExecuted = allFineTunes.Runnings.ToList();
//            inExecutionOrExecuted.AddRange(allFineTunes.Succeeded);
//            inExecutionOrExecuted.AddRange(allFineTunes.ValidatingFiles);
//            Assert.NotEmpty(inExecutionOrExecuted);

//            foreach (var fineTune in inExecutionOrExecuted)
//            {
//                var fineTuneId = fineTune.Id;
//                var retrieveFineTune = await openAiApi.FineTune.RetrieveAsync(fineTuneId);
//                Assert.NotNull(retrieveFineTune);

//                var events = await openAiApi.FineTune.ListEventsAsync(fineTuneId);
//                Assert.NotNull(events);

//                var theEvents = new List<FineTuneEvent>();
//                await foreach (var theEvent in openAiApi.FineTune.ListEventsAsStreamAsync(fineTuneId))
//                {
//                    theEvents.Add(theEvent);
//                }

//                var cancelResult = await openAiApi.FineTune.CancelAsync(fineTuneId);
//                Assert.NotNull(cancelResult);

//                await Task.Delay(5_000);
//                try
//                {
//                    var deleteResult = await openAiApi.File.DeleteAsync(uploadResult.Id);
//                    if (name == "")
//                        Assert.True(deleteResult.Deleted);
//                }
//                catch (Exception ex)
//                {
//                    if (name == "")
//                        Assert.StartsWith("No such File object:", ex.Message);
//                }
//                try
//                {
//                    var deleteResult2 = await openAiApi.File.DeleteAsync(uploadResult2.Id);
//                    if (name == "")
//                        Assert.True(deleteResult2.Deleted);
//                }
//                catch (Exception ex)
//                {
//                    if (name == "")
//                        Assert.StartsWith("No such File object:", ex.Message);
//                }
//            }
//        }
//        [Theory]
//        [InlineData("")]
//        [InlineData("Azure2")]
//        public async ValueTask GetTunesAsStreamAsync(string name)
//        {
//            var openAiApi = _openAiFactory.Create(name);
//            var allFineTunes = await openAiApi.FineTune.ListAsync();
//            var inExecutionOrExecuted = allFineTunes.Runnings.ToList();
//            inExecutionOrExecuted.AddRange(allFineTunes.Succeeded);
//            inExecutionOrExecuted.AddRange(allFineTunes.ValidatingFiles);
//            Assert.NotEmpty(inExecutionOrExecuted);

//            var fineTunes = new List<FineTuneResult>();
//            await foreach (var theEvent in openAiApi.FineTune.ListAsStreamAsync())
//            {
//                fineTunes.Add(theEvent);
//            }

//            foreach (var fineTune in fineTunes)
//            {
//                var fineTuneId = fineTune.Id;

//                var actualEvents = await openAiApi.FineTune.ListEventsAsync(fineTuneId);
//                var theEvents = new List<FineTuneEvent>();
//                await foreach (var theEvent in openAiApi.FineTune.ListEventsAsStreamAsync(fineTuneId))
//                {
//                    theEvents.Add(theEvent);
//                }
//                Assert.Equal(actualEvents.Data.Count, theEvents.Count);
//            }
//        }
//    }
//}
