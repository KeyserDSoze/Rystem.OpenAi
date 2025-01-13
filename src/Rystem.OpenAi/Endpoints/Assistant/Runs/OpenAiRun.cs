using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.FineTune;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiRun : OpenAiBuilder<IOpenAiRun, RunRequest, ChatModelName>, IOpenAiRun
    {
        private string? _threadId = null;
        private Dictionary<string, string>? _fileSearchIncludingQuerystring = null;
        private const string RunsPath = "/runs";
        public OpenAiRun(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLogger logger)
            : base(factory, configurationFactory, logger, OpenAiType.Thread)
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
        public IOpenAiRun WithThread(string threadId)
        {
            _threadId = threadId;
            return this;
        }
        public MessageThreadBuilder<IOpenAiRun> WithThread()
        {
            Request.Thread ??= new();
            return new MessageThreadBuilder<IOpenAiRun>(this, Request.Thread);
        }
        public IOpenAiRun IncludeFileSearchContext()
        {
            _fileSearchIncludingQuerystring ??= [];
            _fileSearchIncludingQuerystring.TryAdd("include[]", "step_details.tool_calls[*].file_search.results[*].content");
            return this;
        }
        public IOpenAiRun ExcludeFileSearchContext()
        {
            _fileSearchIncludingQuerystring?.Remove("include[]");
            if (_fileSearchIncludingQuerystring?.Count == 0)
                _fileSearchIncludingQuerystring = null;
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
        private const string ThreadIdParameterName = "ThreadId";
        private const string ThreadIdParameterMessage = "Thread id or Thread value is null. Please use WithThread method before the request.";

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
        public ValueTask<RunResult> StartAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            CheckThreadId();
            Request.AssistantId = assistantId ?? throw new ArgumentNullException(nameof(assistantId), "Assistant id is null.");
            Request.Stream = false;
            return DefaultServices.HttpClientWrapper.
                PostAsync<RunResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, _threadId == null ? RunsPath : $"/{_threadId}{RunsPath}", _fileSearchIncludingQuerystring),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        public IAsyncEnumerable<AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>> StreamAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            CheckThreadId();
            Request.AssistantId = assistantId ?? throw new ArgumentNullException(nameof(assistantId), "Assistant id is null.");
            Request.Stream = true;
            return DefaultServices.HttpClientWrapper.
                StreamAsync(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, _threadId == null ? RunsPath : $"/{_threadId}{RunsPath}", _fileSearchIncludingQuerystring),
                        Request,
                        HttpMethod.Post,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        ReadRunStepStreamAsync,
                        cancellationToken);
        }
        private static IAsyncEnumerable<AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>> ReadRunStepStreamAsync(Stream stream, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return ReadStreamAsync(stream, response, bufferAsString =>
            {
                try
                {
                    var chunkResponse = bufferAsString.FromJson<AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>>();
                    return chunkResponse!;
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                    return new RunResult();
                }
            }, cancellationToken);
        }
        private static async IAsyncEnumerable<AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>> ReadStreamAsync(Stream stream, HttpResponseMessage _, Func<string, AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>> entityReader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (line.StartsWith(AssistantRunConstants.Streaming.StartingWith))
                    line = line[AssistantRunConstants.Streaming.StartingWith.Length..];
                if (line == AssistantRunConstants.Streaming.Done)
                {
                    yield break;
                }
                else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(AssistantRunConstants.Streaming.Event))
                {
                    yield return entityReader(line);
                }
            }
        }
        public ValueTask<RunResult> CancelAsync(string id, CancellationToken cancellationToken = default)
        {
            CheckThreadId();
            return DefaultServices.HttpClientWrapper.
                PostAsync<RunResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}/cancel", null),
                        null,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<RunResult>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
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
                GetAsync<ResponseAsArray<RunResult>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs", querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        public ValueTask<RunResult> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            CheckThreadId();
            return DefaultServices.HttpClientWrapper.
                GetAsync<RunResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }

        public ValueTask<RunResult> UpdateAsync(string id, CancellationToken cancellationToken = default)
        {
            CheckThreadId();
            return DefaultServices.HttpClientWrapper.
                PostAsync<RunResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}", null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        private ToolOutputRequest? _toolOutputRequest;
        public ToolOutputBuilder<IOpenAiRun> AddToolOutput()
        {
            _toolOutputRequest ??= new ToolOutputRequest();
            return new ToolOutputBuilder<IOpenAiRun>(this, _toolOutputRequest);
        }
        public ValueTask<RunResult> ContinueAsync(string id, CancellationToken cancellationToken)
        {
            CheckThreadId();
            Request.Stream = false;
            return DefaultServices.HttpClientWrapper.
                PostAsync<RunResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}/submit_tool_outputs", _fileSearchIncludingQuerystring),
                        _toolOutputRequest,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        public IAsyncEnumerable<AnyOf<RunResult, ThreadMessageResponse, ThreadChunkMessageResponse>> ContinueStreamAsync(string id, CancellationToken cancellationToken)
        {
            CheckThreadId();
            Request.Stream = true;
            return DefaultServices.HttpClientWrapper.
                StreamAsync(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}/submit_tool_outputs", _fileSearchIncludingQuerystring),
                        _toolOutputRequest,
                        HttpMethod.Post,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        ReadRunStepStreamAsync,
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<RunStepResult>> ListStepsAsync(string id, int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() },
                { "order", order == AssistantOrder.Descending ? "desc" : "asc" },
            };
            if (_fileSearchIncludingQuerystring?.Count > 0)
            {
                foreach (var item in _fileSearchIncludingQuerystring)
                    querystring.Add(item.Key, item.Value);
            }
            if (elementId != null && getAfterTheElementId)
                querystring.Add("after", elementId);
            else if (elementId != null && !getAfterTheElementId)
                querystring.Add("before", elementId);
            CheckThreadId();
            return DefaultServices.HttpClientWrapper.
                GetAsync<ResponseAsArray<RunStepResult>>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{id}/steps", querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        public ValueTask<RunStepResult> GetStepAsync(string runId, string id, CancellationToken cancellationToken)
        {
            CheckThreadId();
            return DefaultServices.HttpClientWrapper.
                GetAsync<RunStepResult>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Thread, _version, null, $"/{_threadId}/runs/{runId}/steps/{id}", _fileSearchIncludingQuerystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }
        private void CheckThreadId()
        {
            if (_threadId == null && Request.Thread == null)
                throw new ArgumentNullException(ThreadIdParameterName, ThreadIdParameterMessage);
        }
    }
}
