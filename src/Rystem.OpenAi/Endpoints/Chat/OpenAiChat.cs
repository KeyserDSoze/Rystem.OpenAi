using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChat : OpenAiBuilder<IOpenAiChat, ChatRequest, ChatModelName>, IOpenAiChat
    {
        private const string FunctionType = "function";

        public OpenAiChat(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.Chat)
        {
            Request.Model = ChatModelName.Gpt4_o;
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Chat != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Chat.Invoke(this);
            }
        }
        private void AddUsages(ChatUsage usage)
        {
            Usages.AddRange([
                new OpenAiCost { Units = usage.PromptTokens, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = usage.CompletionTokensDetails?.AcceptedPredictionTokens ?? 0, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = usage.CompletionTokens, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens }]);
        }
        public async ValueTask<ChatResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.Stream = false;
            Request.StreamOptions = null;
            var response = await DefaultServices.HttpClientWrapper.PostAsync<ChatResult>(DefaultServices.Configuration.GetUri(OpenAiType.Chat, Request.Model!, Forced, string.Empty, null), Request, null, DefaultServices.Configuration, cancellationToken);
            if (response.Usage != null)
                AddUsages(response.Usage);
            return response;
        }
        public async IAsyncEnumerable<ChunkChatResult> ExecuteAsStreamAsync(
            bool withUsage = true,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Request.Stream = true;
            if (withUsage)
                Request.StreamOptions = new StreamOptionsChatRequest
                {
                    IncludeUsage = true
                };
            await foreach (var result in DefaultServices.HttpClientWrapper.StreamAsync<ChunkChatResult>(DefaultServices.Configuration.GetUri(OpenAiType.Chat, Request.Model!, Forced, string.Empty, null), Request, HttpMethod.Post, DefaultServices.Configuration, null, cancellationToken))
            {
                if (result.Usage != null)
                    AddUsages(result.Usage);
                yield return result;
            }
        }
        public IOpenAiChat AddMessage(ChatMessageRequest message)
        {
            Request.Messages ??= [];
            Request.Messages.Add(message);
            return this;
        }
        public IOpenAiChat AddMessages(params ChatMessageRequest[] messages)
        {
            Request.Messages ??= [];
            foreach (var message in messages)
                Request.Messages.Add(message);
            return this;
        }
        public List<ChatMessageRequest> GetCurrentMessages()
            => Request.Messages ?? [];
        public IOpenAiChat AddMessage(string content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessageRequest { Content = content, Role = role });
        public IOpenAiChat AddMessage(ChatMessageContent content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessageRequest { Content = new List<ChatMessageContent> { content }, Role = role });
        public ChatMessageContentBuilder AddContent(ChatRole role = ChatRole.User)
            => new(this, role);
        public IOpenAiChat AddUserMessage(string content)
            => AddMessage(new ChatMessageRequest { Content = content, Role = ChatRole.User });
        public IOpenAiChat AddDeveloperMessage(string content)
            => AddMessage(new ChatMessageRequest { Content = content, Role = ChatRole.Developer });
        public IOpenAiChat AddToolMessage(string functionName, string content)
            => AddMessage(new ChatMessageRequest { Content = content, Role = ChatRole.Tool, ToolCallId = functionName });
        public IOpenAiChat AddUserMessage(ChatMessageContent content)
            => AddMessage(new ChatMessageRequest { Content = new List<ChatMessageContent> { content }, Role = ChatRole.User });
        public ChatMessageContentBuilder AddUserContent()
            => new(this, ChatRole.User);
        public IOpenAiChat AddSystemMessage(string content)
            => AddMessage(new ChatMessageRequest { Content = content, Role = ChatRole.System });
        public IOpenAiChat AddSystemMessage(ChatMessageContent content)
            => AddMessage(new ChatMessageRequest { Content = new List<ChatMessageContent> { content }, Role = ChatRole.System });
        public ChatMessageContentBuilder AddSystemContent()
            => new(this, ChatRole.System);
        public IOpenAiChat AddAssistantMessage(string content)
            => AddMessage(new ChatMessageRequest { Content = content, Role = ChatRole.Assistant });
        public IOpenAiChat AddAssistantMessage(ChatMessageContent content)
            => AddMessage(new ChatMessageRequest { Content = new List<ChatMessageContent> { content }, Role = ChatRole.Assistant });
        public ChatMessageContentBuilder AddAssistantContent()
            => new(this, ChatRole.Assistant);
        public IOpenAiChat WithTemperature(double value)
        {
            if (value < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (value > 2)
                throw new ArgumentException("Temperature with a value greater than 2");
            Request.Temperature = value;
            return this;
        }
        public IOpenAiChat WithNucleusSampling(double value)
        {
            if (value < 0)
                throw new ArgumentException("Nucleus sampling with a value lesser than 0");
            if (value > 1)
                throw new ArgumentException("Nucleus sampling with a value greater than 1");
            Request.TopP = value;
            return this;
        }
        public IOpenAiChat WithNumberOfChoicesPerPrompt(int value)
        {
            Request.NumberOfChoicesPerPrompt = value;
            return this;
        }
        public IOpenAiChat WithStopSequence(params string[] values)
        {
            if (values.Length > 1)
                Request.StopSequence = values;
            else if (values.Length == 1)
                Request.StopSequence = values[0];
            return this;
        }
        public IOpenAiChat AddStopSequence(string value)
        {
            if (Request.StopSequence == null)
                Request.StopSequence = value;
            else if (Request.StopSequence.Is<string>())
                Request.StopSequence = new string[2] { Request.StopSequence.AsT0!, value };
            else if (Request.StopSequence.Is<string[]>())
            {
                var array = Request.StopSequence.AsT1!;
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = value;
                Request.StopSequence = newArray;
            }
            return this;
        }
        public IOpenAiChat SetMaxTokens(int value)
        {
            Request.MaxCompletionsToken = value;
            return this;
        }
        public IOpenAiChat WithPresencePenalty(double value)
        {
            if (value < -2)
                throw new ArgumentException("Presence penalty with a value lesser than -2");
            if (value > 2)
                throw new ArgumentException("Presence penalty with a value greater than 2");
            Request.PresencePenalty = value;
            return this;
        }
        public IOpenAiChat WithFrequencyPenalty(double value)
        {
            if (value < -2)
                throw new ArgumentException("Frequency penalty with a value lesser than -2");
            if (value > 2)
                throw new ArgumentException("Frequency penalty with a value greater than 2");
            Request.FrequencyPenalty = value;
            return this;
        }
        public IOpenAiChat WithBias(string key, int value)
        {
            if (value > 100)
                throw new ArgumentException("Value of bias is greater than 100. Accepted values have range [-100, 100].");
            if (value < -100)
                throw new ArgumentException("Value of bias is lesser than -100. Accepted values have range [-100, 100].");
            Request.Bias ??= [];
            if (!Request.Bias.ContainsKey(key))
                Request.Bias.Add(key, value);
            else
                Request.Bias[key] = value;
            return this;
        }
        public IOpenAiChat WithBias(Dictionary<string, int> bias)
        {
            foreach (var c in bias)
                WithBias(c.Key, c.Value);
            return this;
        }
        public IOpenAiChat WithUser(string user)
        {
            Request.User = user;
            return this;
        }
        public IOpenAiChat WithSeed(int? seed)
        {
            Request.Seed = seed;
            return this;
        }
        public IOpenAiChat ForceResponseFormat(FunctionTool function)
        {
            Request.ResponseFormat = new()
            {
                Content = function,
                Type = ChatConstants.ResponseFormat.JsonSchema
            };
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool
            {
                Function = function
            });
            return this;
        }
        public IOpenAiChat ForceResponseFormat(MethodInfo function)
            => ForceResponseFormat(function.ToFunctionTool(null, true));
        public IOpenAiChat ForceResponseFormat<T>()
            => ForceResponseFormat(typeof(T).ToFunctionTool(typeof(T).Name, null, true));
        public IOpenAiChat ForceResponseAsJsonFormat()
        {
            Request.ResponseFormat = new()
            {
                Type = ChatConstants.ResponseFormat.JsonObject
            };
            return this;
        }
        public IOpenAiChat ForceResponseAsText()
        {
            Request.ResponseFormat = new()
            {
                Type = ChatConstants.ResponseFormat.Text
            };
            return this;
        }
        public IOpenAiChat AvoidCallingTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.None;
            return this;
        }
        public IOpenAiChat ForceCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Required;
            return this;
        }
        public IOpenAiChat CanCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            return this;
        }
        public IOpenAiChat ForceCallFunction(string name)
        {
            Request.ToolChoice = new ForcedFunctionTool
            {
                Type = FunctionType,
                Function = new ForcedFunctionToolName
                {
                    Name = name
                }
            };
            return this;
        }
        public IOpenAiChat AvoidParallelToolCall()
        {
            Request.ParallelToolCalls = false;
            return this;
        }
        public IOpenAiChat ParallelToolCall()
        {
            Request.ParallelToolCalls = true;
            return this;
        }
        public IOpenAiChat WithAutoServiceTier()
        {
            Request.ServiceTier = ChatConstants.ServiceTier.Auto;
            return this;
        }
        public IOpenAiChat WithDefaultServiceTier()
        {
            Request.ServiceTier = ChatConstants.ServiceTier.Default;
            return this;
        }

        public IOpenAiChat ClearTools()
        {
            Request.Tools?.Clear();
            return this;
        }
        public IOpenAiChat AddFunctionTool(FunctionTool tool)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = tool });
            return this;
        }
        public IOpenAiChat AddFunctionTool(MethodInfo function, bool? strict = null)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = function.ToFunctionTool(null, strict) });
            return this;
        }
        public IOpenAiChat AddFunctionTool<T>(string name, string? description = null, bool? strict = null)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = typeof(T).ToFunctionTool(name, description, strict) });
            return this;
        }
    }
}
