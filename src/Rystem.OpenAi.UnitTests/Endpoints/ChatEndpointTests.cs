using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class ChatEndpointTests
    {
        private readonly IFactory<IOpenAi> _openAiFactory;
        public ChatEndpointTests(IFactory<IOpenAi> openAiFactory)
        {
            _openAiFactory = openAiFactory;
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;

            Assert.NotNull(openAiApi.Chat);
            var results = await openAiApi.Chat
                .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelName.Gpt4_o)
                .WithTemperature(1)
                .ExecuteAsync();
            var cost = openAiApi.Chat.CalculateCost();
            Assert.NotNull(results);
            Assert.NotNull(results.CreatedUnixTime);
            Assert.True(results.CreatedUnixTime.Value != 0);
            Assert.NotNull(results.Created);
            Assert.NotNull(results.Choices);
            Assert.True(results.Choices.Count != 0);
            Assert.Contains(results.Choices, c => c.Message?.Role == ChatRole.Assistant);
            Assert.True(cost > 0);
        }


        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            //var biasDictionary = new Dictionary<string, int>
            //{
            //    { "Keystone", -100 },
            //    { "Keystone3", 4 }
            //};
            var results = new List<ChunkChatResult>();
            var chat = openAiApi.Chat;
            if (name != "Azure")
            {
                chat.SetMaxTokens(1200);
            }
            await foreach (var x in chat
                .AddMessage(new ChatMessageRequest { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelName.Gpt4_o)
                .WithTemperature(1)
                .WithStopSequence("alekud")
                .AddStopSequence("coltello")
                .WithNumberOfChoicesPerPrompt(1)
                .WithFrequencyPenalty(0)
                .WithPresencePenalty(0)
                .WithNucleusSampling(1)
                   //.WithBias("Keystone", 4)
                   //.WithBias("Keystone2", 4)
                   //.WithBias(biasDictionary)
                   .WithUser("KeyserDSoze")
                .ExecuteAsStreamAsync())
                results.Add(x);

            Assert.NotEmpty(results);
            Assert.Contains(results, x => x.Choices?.Count != 0);
            Assert.Contains(results.SelectMany(x => x.Choices ?? []), c => c.Delta == null || c.Delta?.Role == ChatRole.Assistant);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithStreamAndCalculateCostAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            var results = new List<ChunkChatResult>();
            await foreach (var x in openAiApi.Chat
                .AddMessage(new ChatMessageRequest { Role = ChatRole.System, Content = "You are a friend of mine." })
                .AddMessage(new ChatMessageRequest { Role = ChatRole.User, Content = "Hello!! How are you?" })
                .WithModel(ChatModelName.Gpt4_o)
                .WithTemperature(1)
                .WithNumberOfChoicesPerPrompt(2)
                .ExecuteAsStreamAsync())
            {
                results.Add(x);
            }
            var cost = openAiApi.Chat.CalculateCost();
            Assert.True(cost > 0);
            Assert.NotEmpty(results);
            var totalResponse = string.Join(string.Empty, results.Select(x => x.Choices?.FirstOrDefault()?.Delta?.Content ?? string.Empty));
            Assert.NotNull(totalResponse);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithFunctionsAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .AddMessage("What is the weather like in Boston?")
                .WithModel(ChatModelName.Gpt4_o)
                .AddFunctionTool(new FunctionTool
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new FunctionToolMainProperty()
                        .AddPrimitive("location", new FunctionToolPrimitiveProperty
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA"
                        })
                        .AddEnum("unit", new FunctionToolEnumProperty
                        {
                            Type = "string",
                            Enums = ["celsius", "fahrenheit"]
                        })
                        .AddRequired("location")
                });

            var response = await request
                .ExecuteAsync();

            var tool = response.Choices?[0]?.Message?.ToolCalls?.First();
            Assert.NotNull(tool?.Function);
            Assert.Equal(tool?.Function.Name, functionName);
            var weatherRequest = tool?.Function.Arguments?.FromJson<WeatherRequest>();
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddSystemMessage("{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            response = await request
                .ExecuteAsync();

            var content = response.Choices?[0]?.Message?.Content;
            Assert.Equal("stop", response.Choices?[0]?.FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithFunctionsAsTAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .AddMessage("What is the weather like in Boston?")
                .WithModel(ChatModelName.Gpt4_o)
                .AddFunctionTool<WeatherRequest>(functionName, "Get the current weather in a given location");

            var response = await request
                .ExecuteAsync();

            var tool = response.Choices?[0]?.Message?.ToolCalls?.First();
            Assert.NotNull(tool?.Function);
            Assert.Equal(tool?.Function.Name, functionName);
            var weatherRequest = tool?.Function.Arguments?.FromJson<WeatherRequest>();
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddSystemMessage("{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            response = await request
                .ExecuteAsync();

            var content = response.Choices?[0]?.Message?.Content;
            Assert.Equal("stop", response.Choices?[0]?.FinishReason);
            Assert.NotNull(content);
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask ForceResponseFormatAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            //var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .AddMessage("What is the weather like in Boston?")
                .WithModel(ChatModelName.Gpt4_o)
                .ForceResponseFormat<WeatherRequest>();

            var response = await request
                .ExecuteAsync();

            var tool = response.Choices?[0]?.Message?.ToolCalls?.First();
            Assert.Null(tool?.Function);
            var weatherRequest = response.Choices?[0]?.Message?.Content?.FromJson<WeatherRequest>();
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddSystemMessage("{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            response = await request
                .ExecuteAsync();

            var content = response.Choices?[0]?.Message?.Content;
            Assert.Equal("stop", response.Choices?[0]?.FinishReason);
            Assert.NotNull(content);
        }
        private sealed class WeatherRequest
        {
            [JsonPropertyName("location")]
            [JsonRequired]
            [Description("The city and state, e.g. San Francisco, CA")]
            public string? Location { get; set; }
            [JsonPropertyName("unit")]
            [Description("Unit Measure of temperature. e.g. Celsius or Fahrenheit")]
            public string? Unit { get; set; }
        }
        [Theory]
        [InlineData("")]
        [InlineData("Azure")]
        public async ValueTask CreateChatCompletionWithFunctionsAsStreamAsync(string name)
        {
            var openAiApi = _openAiFactory.Create(name)!;
            Assert.NotNull(openAiApi.Chat);
            var functionName = "get_current_weather";
            var request = openAiApi.Chat
                .AddUserMessage("What is the weather like in Boston?")
                .WithModel(ChatModelName.Gpt4_o)
                .AddFunctionTool(new FunctionTool
                {
                    Name = functionName,
                    Description = "Get the current weather in a given location",
                    Parameters = new FunctionToolMainProperty()
                        .AddPrimitive("location", new FunctionToolPrimitiveProperty
                        {
                            Type = "string",
                            Description = "The city and state, e.g. San Francisco, CA"
                        })
                        .AddEnum("unit", new FunctionToolEnumProperty
                        {
                            Type = "string",
                            Enums = new List<string> { "celsius", "fahrenheit" }
                        })
                        .AddRequired("location")
                });

            var response = await request
                .ExecuteAsync();

            var function = response.Choices?[0]?.Message?.ToolCalls?.First().Function;
            Assert.NotNull(function);
            Assert.Equal(function.Name, functionName);
            var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(function.Arguments!);
            Assert.NotNull(weatherRequest?.Location);

            request
                .AddSystemMessage("{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            var results = new List<ChunkChatResult>();
            ChunkChatResult? check = null;
            await foreach (var x in request
                .ExecuteAsStreamAsync())
            {
                results.Add(x);
                check = x;
            }
            var finishReason = results.FirstOrDefault(x => x.Choices?.Count > 0 && x.Choices![0].FinishReason != null)?.Choices?[0].FinishReason;
            Assert.Contains(results, x => x.Choices?.Count > 0 && x.Choices![0].Delta?.Content != null);
            Assert.Contains(results, x => x.Choices?.Count > 0 && x.Choices![0].FinishReason != null);
            Assert.Equal("stop", finishReason);
        }
    }
}
