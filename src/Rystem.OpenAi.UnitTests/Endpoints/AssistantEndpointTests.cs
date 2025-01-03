﻿using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Assistant;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class AssistantEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        public AssistantEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("", "gpt-4o")]
        [InlineData("Azure", "gpt-4o-2")]
        public async ValueTask CreateSimpleAssistantFlowAsync(string name, string model)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(model)
                .CreateAsync();
            Assert.NotNull(created);
            Assert.NotNull(created.Id);
            try
            {
                var theAssistant = await assistant.RetrieveAsync(created.Id);
                Assert.NotNull(theAssistant);
                Assert.Equal(created.Id, theAssistant.Id);
                var assistants = await assistant.ListAsync(20);
                Assert.NotNull(assistants?.Data);
                Assert.NotEmpty(assistants.Data);
                var firstAssistant = assistants.Data[0];
                Assert.NotNull(firstAssistant);
                Assert.Equal(created.Id, firstAssistant.Id);
            }
            catch
            {
                throw;
            }
            finally
            {
                var deleted = await assistant.DeleteAsync(created.Id);
                Assert.True(deleted.Deleted);
            }
        }
        [Theory]
        [InlineData("", "gpt-4o")]
        [InlineData("Azure", "gpt-4o-2")]
        public async ValueTask CreateSimpleThreadFlowAsync(string name, string model)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(model)
                .CreateAsync();

            Assert.NotNull(created);
            Assert.NotNull(created.Id);

            var threadClient = openAiApi.Thread;
            var response = await threadClient
                .AddText(Chat.ChatRole.User, "What is 2 + 2?")
                .AddAssistantContent()
                .AddText("What is 4+4?")
                .Builder
                .CreateAsync();

            Assert.NotNull(response);
            Assert.NotNull(response.Id);
            try
            {

                var theThread = await threadClient.RetrieveAsync(response.Id);
                Assert.NotNull(theThread);
                Assert.Equal(response.Id, theThread.Id);

            }
            catch
            {
                throw;
            }
            finally
            {
                var deletion = await threadClient.DeleteAsync(response.Id);
                Assert.True(deletion.Deleted);
                var deleted = await assistant.DeleteAsync(created.Id);
                Assert.True(deleted.Deleted);
            }
        }
        [Theory]
        [InlineData("", "gpt-4o")]
        [InlineData("Azure", "gpt-4o-2")]
        public async ValueTask CreateSimpleThreadFlowAndExecuteASynchronousRunAsync(string name, string model)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(model)
                .CreateAsync();

            Assert.NotNull(created);
            Assert.NotNull(created.Id);

            var threadClient = openAiApi.Thread;
            var response = await threadClient
                .AddText(Chat.ChatRole.User, "What is 2 + 2?")
                .CreateAsync();

            Assert.NotNull(response);
            Assert.NotNull(response.Id);

            try
            {
                var runClient = openAiApi.Run;
                var runResponse = await runClient
                     .WithThread(response.Id)
                     .AddText(Chat.ChatRole.Assistant, "Solve it")
                     .StartAsync(created.Id);
                Assert.NotNull(runResponse);
                Assert.NotNull(runResponse.Id);
                var runs = await runClient.ListAsync(20);
                Assert.NotNull(runs?.Data);
                Assert.NotEmpty(runs.Data);
                var firstRun = runs.Data[0];
                Assert.NotNull(firstRun);
                Assert.Equal(runResponse.Id, firstRun.Id);
                var run = await runClient.RetrieveAsync(runResponse.Id);
                Assert.NotNull(run);
                Assert.Equal(runResponse.Id, run.Id);
                var cancellationResult = await runClient.CancelAsync(runResponse.Id);
                Assert.Equal(RunStatus.Cancelling, cancellationResult.Status);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                var deletion = await threadClient.DeleteAsync(response.Id);
                Assert.True(deletion.Deleted);

                var deleted = await assistant.DeleteAsync(created.Id);
                Assert.True(deleted.Deleted);
            }
        }
        [Theory]
        [InlineData("", "gpt-4o")]
        [InlineData("Azure", "gpt-4o-2")]
        public async ValueTask CreateSimpleThreadFlowAndExecuteASynchronousRunAndReadAllStepsAsync(string name, string model)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(model)
                .CreateAsync();

            Assert.NotNull(created);
            Assert.NotNull(created.Id);

            var threadClient = openAiApi.Thread;
            var response = await threadClient
                .AddText(Chat.ChatRole.User, "What is 2 + 2?")
                .CreateAsync();

            Assert.NotNull(response);
            Assert.NotNull(response.Id);

            var runClient = openAiApi.Run;
            var runResponse = await runClient
                 .WithThread(response.Id)
                 .AddText(Chat.ChatRole.Assistant, "Solve it")
                 .StartAsync(created.Id);
            Assert.NotNull(runResponse);
            Assert.NotNull(runResponse.Id);
            try
            {
                var runs = await runClient.ListAsync(20);
                Assert.NotNull(runs?.Data);
                Assert.NotEmpty(runs.Data);
                var firstRun = runs.Data[0];
                Assert.NotNull(firstRun);
                Assert.Equal(runResponse.Id, firstRun.Id);
                var run = await runClient.RetrieveAsync(runResponse.Id);
                Assert.NotNull(run);
                Assert.Equal(runResponse.Id, run.Id);
                await Task.Delay(4_000);
                var steps = await runClient.ListStepsAsync(runResponse.Id);
                Assert.NotNull(steps);
                Assert.NotNull(steps.Data);
                Assert.NotEmpty(steps.Data);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    var cancellationResult = await runClient.CancelAsync(runResponse.Id);
                    Assert.Equal(RunStatus.Cancelling, cancellationResult.Status);
                }
                catch { }
                var deletion = await threadClient.DeleteAsync(response.Id);
                Assert.True(deletion.Deleted);

                var deleted = await assistant.DeleteAsync(created.Id);
                Assert.True(deleted.Deleted);
            }
        }
    }
}
