using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Test.Functions;
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
                .WithModel(ChatModelType.Gpt35Turbo)
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
            //var biasDictionary = new Dictionary<string, int>
            //{
            //    { "Keystone", -100 },
            //    { "Keystone3", 4 }
            //};
            var results = new List<ChatResult>();
            await foreach (var x in openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel("testModel", ModelFamilyType.Gpt3_5)
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithTemperature(1)
                .WithStopSequence("alekud")
                .AddStopSequence("coltello")
                .WithNumberOfChoicesPerPrompt(1)
                .WithFrequencyPenalty(0)
                .WithPresencePenalty(0)
                .WithNucleusSampling(1)
                .SetMaxTokens(1200)
                   //.WithBias("Keystone", 4)
                   //.WithBias("Keystone2", 4)
                   //.WithBias(biasDictionary)
                   .WithUser("KeyserDSoze")
                .ExecuteAsStreamAsync())
                results.Add(x.LastChunk);

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
            var results = new List<ChatResult>();
            ChatResult check = null;
            var cost = 0M;
            await foreach (var x in openAiApi.Chat
                .Request(new ChatMessage { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessage { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithTemperature(1)
                .WithNumberOfChoicesPerPrompt(2)
                .ExecuteAsStreamAndCalculateCostAsync())
            {
                results.Add(x.Result.LastChunk);
                check = x.Result.Composed;
                cost += x.CalculateCost();
            }
            Assert.True(cost > 0);
            Assert.NotNull(check.Choices.Last().Message.Content);
            Assert.NotEmpty(results);
            Assert.True(results.Last().Choices.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Delta?.Content != null);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithFunctionsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .RequestWithUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithFunction(new ChatFunction
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new ChatFunctionParameters
                    {
                        Type = "object",
                        Properties = new Dictionary<string, ChatFunctionProperty>
                        {
                            {
                                "location",
                                new ChatFunctionProperty
                                {
                                    Type= "string",
                                    Description = "The city and state, e.g. San Francisco, CA"
                                }
                            },
                            {
                                "unit",
                                new ChatFunctionEnumProperty
                                {
                                    Type= "string",
                                    Enums = new List<string>{ "celsius", "fahrenheit" }
                                }
                            }
                        },
                        Required = new List<string> { "location" }
                    }
                });
            var response = await request
                .ExecuteAndCalculateCostAsync();

            var function = response.Result.Choices[0].Message.Function;
            Assert.NotNull(function);
            Assert.Equal(function.Name, functionName);
            var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(function.Arguments);
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddFunctionMessage(functionName, "{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            response = await request
                .ExecuteAndCalculateCostAsync();

            var content = response.Result.Choices[0].Message.Content;
            Assert.NotNull(content);

        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithComplexFunctionsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var response = await openAiApi.Chat
                .RequestWithUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithFunction(WeatherFunction.NameLabel)
                .ExecuteAndCalculateCostAsync(true);

            var content = response.Result.Choices[0].Message.Content;
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamAndCalculateCostAndComplexFunctionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var results = new List<ChatResult>();
            ChatResult check = null;
            var cost = 0M;
            await foreach (var x in openAiApi.Chat
                .RequestWithUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsStreamAndCalculateCostAsync(true))
            {
                results.Add(x.Result.LastChunk);
                check = x.Result.Composed;
                cost += x.CalculateCost();
            }
            Assert.True(cost > 0);
            Assert.NotNull(check.Choices.Last().Message.Content);
            Assert.NotEmpty(results);
            Assert.True(results.Last().Choices.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Delta?.Content != null);
        }
    }
}
