using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Assistant;
using Rystem.OpenAi.Audio;
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
            var theAssistant = await assistant.RetrieveAsync(created.Id);
            Assert.NotNull(theAssistant);
            Assert.Equal(created.Id, theAssistant.Id);
            var assistants = await assistant.ListAsync(20);
            Assert.NotNull(assistants?.Data);
            Assert.NotEmpty(assistants.Data);
            var firstAssistant = assistants.Data[0];
            Assert.NotNull(firstAssistant);
            Assert.Equal(created.Id, firstAssistant.Id);
            var deleted = await assistant.DeleteAsync(created.Id);
            Assert.True(deleted.Deleted);
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
            var theThread = await threadClient.RetrieveAsync(response.Id);
            Assert.NotNull(theThread);
            Assert.Equal(response.Id, theThread.Id);

            var deletion = await threadClient.DeleteAsync(response.Id);
            Assert.True(deletion.Deleted);

            var deleted = await assistant.DeleteAsync(created.Id);
            Assert.True(deleted.Deleted);
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
    }
}
