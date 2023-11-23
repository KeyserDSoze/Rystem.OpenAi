using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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

            //var chatHistory = openAiApi.Chat.RequestWithSystemMessage("for example something to pass to the system.")
            //    .AddUserMessage("first message from the user");
            //var message = await chatHistory
            //    .ExecuteAsync();
            ////here the first output for the user from openai
            //var contentFromAssistant = message.Choices[0].Message.Content;
            ////response from openai added to the chat builder
            //chatHistory.AddAssistantMessage(contentFromAssistant);
            ////message from the user
            //chatHistory.AddUserMessage("second message from the user");
            //var message2 = await chatHistory
            //   .ExecuteAsync();
            ////here the second output for the user from openai
            //var secondContentFromAssistant = message2.Choices[0].Message.Content;
            ////response from openai added to the chat builder
            //chatHistory.AddAssistantMessage(contentFromAssistant);
            ////message from the user
            //chatHistory.AddUserMessage("third message from the user");
            //var message3 = await chatHistory
            //   .ExecuteAsync();
            ////and so on......

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
                .WithFunction(new JsonFunction
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new JsonFunctionNonPrimitiveProperty()
                        .AddPrimitive("location", new JsonFunctionProperty
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA"
                        })
                        .AddEnum("unit", new JsonFunctionEnumProperty
                        {
                            Type = "string",
                            Enums = new List<string> { "celsius", "fahrenheit" }
                        })
                        .AddRequired("location")
                });

            var response = await request
                .ExecuteAndCalculateCostAsync();

            var function = response.Result.Choices[0].Message.ToolCall?.Function;
            Assert.NotNull(function);
            Assert.Equal(function.Name, functionName);
            var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(function.Arguments);
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddFunctionMessage(functionName, "{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            response = await request
                .ExecuteAndCalculateCostAsync();

            var content = response.Result.Choices[0].Message.Content;
            Assert.Equal("functionExecuted", response.Result.Choices[0].FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithFunctionsAsStreamAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .RequestWithUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelType.Gpt35Turbo)
                .WithFunction(new JsonFunction
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new JsonFunctionNonPrimitiveProperty()
                        .AddPrimitive("location", new JsonFunctionProperty
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA"
                        })
                        .AddEnum("unit", new JsonFunctionEnumProperty
                        {
                            Type = "string",
                            Enums = new List<string> { "celsius", "fahrenheit" }
                        })
                        .AddRequired("location")
                });

            var response = await request
                .ExecuteAndCalculateCostAsync();

            var function = response.Result.Choices[0].Message.ToolCall.Function;
            Assert.NotNull(function);
            Assert.Equal(function.Name, functionName);
            var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(function.Arguments);
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddFunctionMessage(functionName, "{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            var results = new List<ChatResult>();
            ChatResult check = null;
            await foreach (var x in request
                .ExecuteAsStreamAndCalculateCostAsync())
            {
                results.Add(x.Result.LastChunk);
                check = x.Result.Composed;
            }

            var content = check.Choices[0].Message.Content;
            var finishReason = check.Choices[0].FinishReason;
            Assert.Equal("functionExecuted", finishReason);
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
            Assert.Equal("functionAutoExecuted", response.Result.Choices[0].FinishReason);
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
            Assert.Equal("functionAutoExecuted", results.Last().Choices.Last().FinishReason);
            Assert.Contains(results.SelectMany(x => x.Choices), c => c.Delta?.Content != null);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithComplexFunctionsAndEnumAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var response = await openAiApi.Chat
                .RequestWithUserMessage("Where is my car DDM3YAA?")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsync(true);

            var content = response.Choices[0].Message.Content;
            Assert.Equal("functionAutoExecuted", response.Choices[0].FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithComplexFunctionsAndComplexObjectsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var response = await openAiApi.Chat
                .RequestWithUserMessage("I want to travel in India from Rome, and I have a budget of 350 dollars. Does a fly exist?")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsync(true);

            var content = response.Choices[0].Message.Content;
            Assert.Equal("functionAutoExecuted", response.Choices[0].FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithComplexFunctionsAndArraysAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            var results = new List<ChatResult>();
            ChatResult check = null;
            await foreach (var x in openAiApi.Chat
                .RequestWithUserMessage("I need to buy 3 apples, 2 bananas, chicken, salmon and tuna. What is the best price to buy them all?")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsStreamAsync(true))
            {
                results.Add(x.LastChunk);
                check = x.Composed;
            }

            var content = check.Choices[0].Message.Content;
            Assert.Equal("functionAutoExecuted", check.Choices[0].FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithNullFunctionsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var response = await openAiApi.Chat
                .RequestWithUserMessage("My username is Keyser D. Soze and I want to know what I have in my cart.")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsync(true);

            var function = response.Choices[0].Message.ToolCall.Function;
            Assert.NotNull(function);
            Assert.Contains("Keyser D. Soze", function.Arguments);
            Assert.Equal("null", response.Choices[0].FinishReason);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamWithNullFunctionsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name);
            Assert.NotNull(openAiApi.Chat);
            var results = new List<ChatResult>();
            ChatResult check = null;
            await foreach (var x in openAiApi.Chat
                .RequestWithUserMessage("My username is Keyser D. Soze and I want to know what I have in my cart.")
                .WithModel(ChatModelType.Gpt35Turbo_Snapshot)
                .WithAllFunctions()
                .ExecuteAsStreamAsync(true))
            {
                results.Add(x.LastChunk);
                check = x.Composed;
            }
            var function = check.Choices[0].Message.ToolCall.Function;
            Assert.NotNull(function);
            Assert.Contains("Keyser D. Soze", function.Arguments);
            Assert.Equal("null", check.Choices[0].FinishReason);
        }
    }
}
