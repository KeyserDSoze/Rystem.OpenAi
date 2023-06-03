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
        private readonly IOpenAiFactory _openAiFactory;
        public ChatEndpointTests(IOpenAiFactory openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);

            Assert.NotNull(openAiApi.Chat);
            var results = await openAiApi.Chat
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


        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var biasDictionary = new Dictionary<string, int>
            {
                { "Keystone", 1 },
                { "Keystone3", 4 }
            };
            var results = new List<ChatResult>();
            await foreach (var x in openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel("testModel", ModelFamilyType.Gpt3_5)
                .WithModel(ChatModelType.Gpt35Turbo0301)
                .WithTemperature(1)
                .WithStopSequence("alekud")
                .AddStopSequence("coltello")
                .WithNumberOfChoicesPerPrompt(1)
                .WithFrequencyPenalty(0)
                .WithPresencePenalty(0)
                .WithNucleusSampling(1)
                .SetMaxTokens(1200)
                   .WithBias("Keystone", 4)
                   .WithBias("Keystone2", 4)
                   .WithBias(biasDictionary)
                   .WithUser("KeyserDSoze")
                .ExecuteAsStreamAsync())
                results.Add(x);

            Assert.NotEmpty(results);
            Assert.True(results.Last().Choices.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Message == null || c.Message?.Role == ChatRole.Assistant);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamAndCalculateCostAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var biasDictionary = new Dictionary<string, int>
            {
                { "Keystone", 1 },
                { "Keystone3", 4 }
            };
            var results = new List<ChatResult>();
            await foreach (var x in openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelType.Gpt35Turbo0301)
                .WithTemperature(1)
                .ExecuteAsStreamAndCalculateCostAsync())
            {
                results.Add(x.Result);
            }

            Assert.NotEmpty(results);
            Assert.True(results.Last().Choices.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Message == null || c.Message?.Role == ChatRole.Assistant);
        }
    }
}
