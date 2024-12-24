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
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateSimpleAssistantFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(ChatModelName.Gpt4_o)
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
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateSimpleThreadFlowAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Assistant);
            var assistant = openAiApi.Assistant;
            var created = await assistant
                .WithTemperature(0.5)
                .WithInstructions("You are a personal math tutor. When asked a question, write and run Python code to answer the question.")
                .WithCodeInterpreter()
                .WithModel(ChatModelName.Gpt4_o)
                .CreateAsync();

            Assert.NotNull(created);
            Assert.NotNull(created.Id);

            var threadClient = openAiApi.Thread;
            threadClient.AddText(Chat.ChatRole.User, "What is 2 + 2?");

            var deleted = await assistant.DeleteAsync(created.Id);
            Assert.True(deleted.Deleted);
        }
    }
}
