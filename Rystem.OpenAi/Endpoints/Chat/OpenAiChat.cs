using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChat : OpenAiBuilder<IOpenAiChat, ChatRequest, ChatUsage>, IOpenAiChat
    {
        public OpenAiChat(IFactory<DefaultServices> factory) : base(factory)
        {
        }
        public async ValueTask<ChatResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.Stream = false;
            Request.StreamOptions = null;
            var response = await DefaultServices.HttpClient.PostAsync<ChatResult>(DefaultServices.Configuration.GetUri(OpenAiType.Chat, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
            if (response.Usage != null)
                Usages.Add(response.Usage);
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
            await foreach (var result in DefaultServices.HttpClient.StreamAsync<ChunkChatResult>(DefaultServices.Configuration.GetUri(OpenAiType.Chat, Request.Model!, Forced, string.Empty), Request, HttpMethod.Post, DefaultServices.Configuration, null, cancellationToken))
            {
                if (result.Usage != null)
                    Usages.Add(result.Usage);
                yield return result;
            }
        }
        public IOpenAiChat AddMessage(ChatMessage message)
        {
            Request.Messages ??= [];
            Request.Messages.Add(message);
            return this;
        }
        public IOpenAiChat AddMessages(params ChatMessage[] messages)
        {
            Request.Messages ??= [];
            foreach (var message in messages)
                Request.Messages.Add(message);
            return this;
        }
        public List<ChatMessage> GetCurrentMessages()
            => Request.Messages ?? [];
        public IOpenAiChat AddMessage(string content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessage { Content = content, Role = role });
        public IOpenAiChat AddMessage(ChatMessageContent content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = role });
        public ChatMessageContentBuilder AddContent(ChatRole role = ChatRole.User)
            => new(this, role);
        public IOpenAiChat AddUserMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.User });
        public IOpenAiChat AddUserMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.User });
        public ChatMessageContentBuilder AddUserContent()
            => new(this, ChatRole.User);
        public IOpenAiChat AddSystemMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.System });
        public IOpenAiChat AddSystemMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.System });
        public ChatMessageContentBuilder AddSystemContent()
            => new ChatMessageContentBuilder(this, ChatRole.System);
        public IOpenAiChat AddAssistantMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.Assistant });
        public IOpenAiChat AddAssistantMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.Assistant });
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
            else if (Request.StopSequence is string stringableSequence)
                Request.StopSequence = new string[2] { stringableSequence, value };
            else if (Request.StopSequence is string[] array)
            {
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
            Request.Bias ??= new Dictionary<string, int>();
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
        public decimal CalculateCost()
        {
            decimal outputPrice = 0;
            foreach (var responses in Usages)
            {
                outputPrice += DefaultServices.Price.CalculatePrice(Request.Model!,
                    new OpenAiCost { Value = responses.PromptTokens, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                    new OpenAiCost { Value = responses.CompletionTokensDetails?.AcceptedPredictionTokens ?? 0, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                    new OpenAiCost { Value = responses.CompletionTokens, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens });
            }
            return outputPrice;
        }
        public IOpenAiChat WithSeed(int? seed)
        {
            Request.Seed = seed;
            return this;
        }
        public IOpenAiChat WithFunction(FunctionTool function)
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool
            {
                Function = function
            });
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
        public IOpenAiChat ForceCallFunction()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Required;
            return this;
        }
        public IOpenAiChat ForceCallFunction(ForcedFunctionTool forcedFunction)
        {
            Request.ToolChoice = forcedFunction;
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
    }
}
