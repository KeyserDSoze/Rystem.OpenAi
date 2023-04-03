using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ChatEndpointTests
    {
        private readonly IOpenAiApi _openAiApi;
        public ChatEndpointTests(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        [Fact]
        public async ValueTask CreateChatCompletionAsync()
        {
            Assert.NotNull(_openAiApi.Chat);

            var results = await _openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelType.Gpt35Turbo0301)
                .WithTemperature(1)
                .ExecuteAsync();

            Assert.NotNull(results);
            Assert.NotNull(results.CreatedUnixTime);
            Assert.True(results.CreatedUnixTime.Value != 0);
            Assert.NotNull(results.Created);
            Assert.NotNull(results.Choices);
            Assert.True(results.Choices.Count != 0);
            Assert.Contains(results.Choices, c => c.Message.Role == ChatRole.Assistant);
        }


        [Fact]
        public async ValueTask CreateChatCompletionWithStreamAsync()
        {
            Assert.NotNull(_openAiApi.Chat);

            var results = new List<ChatResult>();
            await foreach (var x in _openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelType.Gpt35Turbo0301)
                .WithTemperature(1)
                .ExecuteAsStreamAsync())
                results.Add(x);

            Assert.NotEmpty(results);
            Assert.True(results.Last().Choices.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Message == null || c.Message?.Role == ChatRole.Assistant);
        }
    }
}
