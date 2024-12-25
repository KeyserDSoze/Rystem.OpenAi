using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunRequest : AssistantRequest
    {
        [JsonIgnore]
        public StringBuilder? AdditionalInstructionsBuilder { get; set; }
        [JsonPropertyName("additional_instructions")]
        public string? AdditionalInstructions { get => AdditionalInstructionsBuilder?.ToString(); set => AdditionalInstructionsBuilder = new(value); }
        [JsonPropertyName("additional_messages")]
        public List<ThreadMessage>? AdditionalMessages { get; set; }
        [JsonPropertyName("stream")]
        public bool Stream { get; set; }
        [JsonPropertyName("max_prompt_tokens")]
        public int? MaxPromptTokens { get; set; }
        [JsonPropertyName("max_completion_tokens")]
        public int? MaxCompletionTokens { get; set; }
        [JsonPropertyName("truncation_strategy")]
        public RunTruncationStrategy? RunTruncationStrategy { get; set; }
        [JsonPropertyName("tool_choice")]
        public AnyOf<string, ForcedFunctionTool>? ToolChoice { get; set; }
        [JsonPropertyName("parallel_tool_calls")]
        public bool? ParallelToolCalls { get; set; }
    }
    public interface IOpenAiRun : IOpenAiBase<IOpenAiRun, ChatModelName>
    {

    }
    internal sealed class OpenAiRun : OpenAiBuilder<IOpenAiRun, RunRequest, ChatModelName>, IOpenAiRun
    {
        private Dictionary<string, string>? s_fileSearchIncludingQuerystring = null;
        private const string RunsPath = "/runs";
        public OpenAiRun(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.Assistant)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Run != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Run.Invoke(this);
            }
        }
        public IOpenAiRun IncludeFileSearchContext()
        {
            s_fileSearchIncludingQuerystring ??= [];
            s_fileSearchIncludingQuerystring.TryAdd("include[]", "step_details.tool_calls[*].file_search.results[*].content");
            return this;
        }
        public IOpenAiRun ExcludeFileSearchContext()
        {
            s_fileSearchIncludingQuerystring?.Remove("include[]");
            if (s_fileSearchIncludingQuerystring?.Count == 0)
                s_fileSearchIncludingQuerystring = null;
            return this;
        }
        public IOpenAiRun AvoidCallingTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.None;
            return this;
        }
        public IOpenAiRun ForceCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Required;
            return this;
        }
        public IOpenAiRun CanCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            return this;
        }
        private const string FunctionType = "function";


        public IOpenAiRun ForceCallFunction(string name)
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
        public IOpenAiRun AvoidParallelToolCall()
        {
            Request.ParallelToolCalls = false;
            return this;
        }
        public IOpenAiRun ParallelToolCall()
        {
            Request.ParallelToolCalls = true;
            return this;
        }
        public IOpenAiRun SetMaxPromptTokens(int value)
        {
            Request.MaxPromptTokens = value;
            return this;
        }
        public IOpenAiRun SetMaxCompletionTokens(int value)
        {
            Request.MaxCompletionTokens = value;
            return this;
        }
        public IOpenAiRun ForceResponseFormat(FunctionTool function)
        {
            Request.ResponseFormat = new ResponseFormatChatRequest()
            {
                Content = function,
                Type = ChatConstants.ResponseFormat.JsonSchema
            };
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool
            {
                Function = function
            });
            return this;
        }
        public IOpenAiRun ForceResponseFormat(MethodInfo function)
            => ForceResponseFormat(function.ToFunctionTool(null, true));
        public IOpenAiRun ForceResponseFormat<T>()
            => ForceResponseFormat(typeof(T).ToFunctionTool(typeof(T).Name, null, true));
        public IOpenAiRun ForceResponseAsJsonFormat()
        {
            Request.ResponseFormat = new ResponseFormatChatRequest()
            {
                Type = ChatConstants.ResponseFormat.JsonObject
            };
            return this;
        }
        public IOpenAiRun ForceResponseAsText()
        {
            Request.ResponseFormat = new ResponseFormatChatRequest()
            {
                Type = ChatConstants.ResponseFormat.Text
            };
            return this;
        }
        private ThreadMessage GetLastMessage(ChatRole role)
        {
            Request.AdditionalMessages ??= [];
            var lastMessage = Request.AdditionalMessages.LastOrDefault();
            if (lastMessage?.Role != role.AsString())
            {
                lastMessage = new ThreadMessage
                {
                    Role = role.AsString(),
                };
                Request.AdditionalMessages.Add(lastMessage);
            }
            return lastMessage;
        }
        private List<ChatMessageContent> GetLastContent(ChatRole role)
        {
            var lastMessage = GetLastMessage(role);
            lastMessage.Content ??= new List<ChatMessageContent>();
            return lastMessage.Content.CastT1;
        }
        public IOpenAiRun AddText(ChatRole role, string text)
        {
            var contents = GetLastContent(role);
            contents.Add(new ChatMessageContent { Text = text, Type = ChatConstants.ContentType.Text });
            return this;
        }
        public ChatMessageContentBuilder<IOpenAiRun> AddContent(ChatRole role = ChatRole.User)
        {
            var content = new List<ChatMessageContent>();
            Request.AdditionalMessages ??= [];
            Request.AdditionalMessages.Add(new ThreadMessage { Content = content, Role = role.AsString() });
            return new ChatMessageContentBuilder<IOpenAiRun>(this, content);
        }
        public ChatMessageContentBuilder<IOpenAiRun> AddUserContent()
          => AddContent(ChatRole.User);
        public ChatMessageContentBuilder<IOpenAiRun> AddAssistantContent()
          => AddContent(ChatRole.Assistant);
        public IOpenAiRun AddAttachment(ChatRole role, string? fileId, bool withCodeInterpreter = false, bool withFileSearch = false)
        {
            var message = GetLastMessage(role);
            message.Attachments ??= [];
            var attachment = new ThreadAttachment
            {
                FileId = fileId,
            };
            if (withCodeInterpreter || withFileSearch)
            {
                attachment.Tools = [];
                if (withCodeInterpreter)
                    attachment.Tools.Add(ThreadAttachmentTool.CodeInterpreter);
                if (withFileSearch)
                    attachment.Tools.Add(ThreadAttachmentTool.FileSearch);
            }
            message.Attachments.Add(attachment);
            return this;
        }
        public IOpenAiRun AddMetadata(ChatRole role, string key, string value)
        {
            var message = GetLastMessage(role);
            message.Metadata ??= [];
            message.Metadata.TryAdd(key, value);
            return this;
        }
        public IOpenAiRun ClearTools()
        {
            Request.Tools?.Clear();
            return this;
        }
        public IOpenAiRun AddFunctionTool(FunctionTool tool)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = tool });
            return this;
        }
        public IOpenAiRun AddFunctionTool(MethodInfo function, bool? strict = null)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = function.ToFunctionTool(null, strict) });
            return this;
        }
        public IOpenAiRun AddFunctionTool<T>(string name, string? description = null, bool? strict = null)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = typeof(T).ToFunctionTool(name, description, strict) });
            return this;
        }
        public IOpenAiRun AddMetadata(string key, string value)
        {
            if (Request.Metadata == null)
                Request.Metadata = [];
            if (!Request.Metadata.TryAdd(key, value))
                Request.Metadata[key] = value;
            return this;
        }

        public IOpenAiRun AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            return this;
        }

        public IOpenAiRun ClearMetadata()
        {
            Request.Metadata?.Clear();
            return this;
        }
        public IOpenAiRun RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            return this;
        }

        public IOpenAiRun WithCodeInterpreter()
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantCodeInterpreterTool());
            return this;
        }
        public IOpenAiRun WithDescription(string description)
        {
            Request.Description = description;
            return this;
        }

        public IOpenAiFileSearchAssistant<IOpenAiRun> WithFileSearch(int maxNumberOfResults = 20)
        {
            if (maxNumberOfResults < 1)
                throw new ArgumentException("Max number of results with a value lesser than 1");
            if (maxNumberOfResults > 50)
                throw new ArgumentException("Max number of results with a value greater than 50");
            var fileSearch = new AssistantFileSearchTool
            {
                FileSearch = new()
                {
                    MaxNumberOfResults = maxNumberOfResults
                }
            };
            Request.Tools ??= [];
            Request.Tools.Add(fileSearch);
            return new OpenAiFileSearchAssistant<IOpenAiRun>(this, fileSearch);
        }

        public IOpenAiRun OverrideAssistantInstructions(string text)
        {
            Request.InstructionsBuilder ??= new();
            Request.InstructionsBuilder.Append(text);
            return this;
        }
        public IOpenAiRun AddFurtherInstructions(string text)
        {
            Request.AdditionalInstructionsBuilder ??= new();
            Request.AdditionalInstructionsBuilder.Append(text);
            return this;
        }

        public IOpenAiRun WithName(string name)
        {
            Request.Name = name;
            return this;
        }

        public IOpenAiRun WithTemperature(double value)
        {
            if (value < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (value > 2)
                throw new ArgumentException("Temperature with a value greater than 2");
            Request.Temperature = value;
            return this;
        }
        public IOpenAiRun WithNucleusSampling(double value)
        {
            if (value < 0)
                throw new ArgumentException("Nucleus sampling with a value lesser than 0");
            if (value > 1)
                throw new ArgumentException("Nucleus sampling with a value greater than 1");
            Request.TopP = value;
            return this;
        }

        public IOpenAiToolResourcesAssistant<IOpenAiRun> WithToolResources()
        {
            Request.ToolResources ??= new();
            return new OpenAiToolResourcesAssistant<IOpenAiRun>(this, Request.ToolResources);
        }
        public IOpenAiRun WithAutoTruncation()
        {
            Request.RunTruncationStrategy = RunTruncationStrategy.Auto;
            return this;
        }
        public IOpenAiRun WithTruncationOnLastMessages(int numberOfMaximumLastMessagesInTheContext)
        {
            Request.RunTruncationStrategy = RunTruncationStrategy.LastMessages(numberOfMaximumLastMessagesInTheContext);
            return this;
        }

        private static readonly Dictionary<string, string> s_betaHeaders = new()
        {
            { "OpenAI-Beta", "assistants=v2" }
        };
        private ValueTask<AssistantRequest> CreateAsync(string? threadId, bool streaming, CancellationToken cancellationToken)
        {
            Request.Stream = streaming;
            return DefaultServices.HttpClientWrapper.
                PostAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, string.Empty, Forced, threadId == null ? RunsPath : $"/{threadId}{RunsPath}", s_fileSearchIncludingQuerystring),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<AssistantRequest> CreateAsync(string? threadId = null, CancellationToken cancellationToken = default)
            => CreateAsync(threadId, false, cancellationToken);
        public ValueTask<AssistantRequest> CreateAsStreamAsync(string? threadId = null, CancellationToken cancellationToken = default)
            => CreateAsync(threadId, true, cancellationToken);
        public ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<AssistantRequest>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() },
                { "order", order == AssistantOrder.Descending ? "desc" : "asc" },
            };
            if (elementId != null && getAfterTheElementId)
                querystring.Add("after", elementId);
            else if (elementId != null && !getAfterTheElementId)
                querystring.Add("before", elementId);
            return DefaultServices.HttpClientWrapper.
                GetAsync<ResponseAsArray<AssistantRequest>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, string.Empty, Forced, string.Empty, querystring),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
        public ValueTask<AssistantRequest> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                GetAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, string.Empty, Forced, $"/{id}", null),
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }

        public ValueTask<AssistantRequest> UpdateAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, string.Empty, Forced, $"/{id}", null),
                        Request,
                        s_betaHeaders,
                        DefaultServices.Configuration,
                        cancellationToken);
        }
    }
}
