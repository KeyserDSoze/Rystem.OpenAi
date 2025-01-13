using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Assistant
{
    internal sealed class OpenAiAssistant : OpenAiBuilder<IOpenAiAssistant, AssistantRequest, ChatModelName>, IOpenAiAssistant
    {
        public OpenAiAssistant(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLogger logger)
            : base(factory, configurationFactory, logger, OpenAiType.Assistant)
        {
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Assistant != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Assistant.Invoke(this);
            }
        }
        public IOpenAiAssistant ForceResponseFormat(FunctionTool function)
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
        public IOpenAiAssistant ForceResponseFormat(MethodInfo function)
            => ForceResponseFormat(function.ToFunctionTool(null, true));
        public IOpenAiAssistant ForceResponseFormat<T>()
            => ForceResponseFormat(typeof(T).ToFunctionTool(typeof(T).Name, null, true));
        public IOpenAiAssistant ForceResponseAsJsonFormat()
        {
            Request.ResponseFormat = new ResponseFormatChatRequest()
            {
                Type = ChatConstants.ResponseFormat.JsonObject
            };
            return this;
        }
        public IOpenAiAssistant ForceResponseAsText()
        {
            Request.ResponseFormat = new ResponseFormatChatRequest()
            {
                Type = ChatConstants.ResponseFormat.Text
            };
            return this;
        }
        public IOpenAiAssistant ClearTools()
        {
            Request.Tools?.Clear();
            return this;
        }
        public IOpenAiAssistant AddFunctionTool(FunctionTool tool)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = tool });
            return this;
        }
        public IOpenAiAssistant AddFunctionTool(MethodInfo function, bool? strict = null)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = function.ToFunctionTool(null, strict) });
            return this;
        }
        public IOpenAiAssistant AddFunctionTool<T>(string name, string? description = null, bool? strict = null)
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantFunctionTool { Function = typeof(T).ToFunctionTool(name, description, strict) });
            return this;
        }
        public IOpenAiAssistant AddMetadata(string key, string value)
        {
            if (Request.Metadata == null)
                Request.Metadata = [];
            if (!Request.Metadata.TryAdd(key, value))
                Request.Metadata[key] = value;
            return this;
        }

        public IOpenAiAssistant AddMetadata(Dictionary<string, string> metadata)
        {
            Request.Metadata = metadata;
            return this;
        }

        public IOpenAiAssistant ClearMetadata()
        {
            Request.Metadata?.Clear();
            return this;
        }


        public IOpenAiAssistant RemoveMetadata(string key)
        {
            if (Request.Metadata?.ContainsKey(key) == true)
                Request.Metadata.Remove(key);
            return this;
        }

        public IOpenAiAssistant WithCodeInterpreter()
        {
            Request.Tools ??= [];
            Request.Tools.Add(new AssistantCodeInterpreterTool());
            return this;
        }
        public IOpenAiAssistant WithDescription(string description)
        {
            Request.Description = description;
            return this;
        }

        public IOpenAiFileSearchAssistant<IOpenAiAssistant> WithFileSearch(int maxNumberOfResults = 20)
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
            return new OpenAiFileSearchAssistant<IOpenAiAssistant>(this, fileSearch);
        }

        public IOpenAiAssistant WithInstructions(string text)
        {
            Request.InstructionsBuilder ??= new();
            Request.InstructionsBuilder.Append(text);
            return this;
        }

        public IOpenAiAssistant WithName(string name)
        {
            Request.Name = name;
            return this;
        }

        public IOpenAiAssistant WithTemperature(double value)
        {
            if (value < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (value > 2)
                throw new ArgumentException("Temperature with a value greater than 2");
            Request.Temperature = value;
            return this;
        }
        public IOpenAiAssistant WithNucleusSampling(double value)
        {
            if (value < 0)
                throw new ArgumentException("Nucleus sampling with a value lesser than 0");
            if (value > 1)
                throw new ArgumentException("Nucleus sampling with a value greater than 1");
            Request.TopP = value;
            return this;
        }

        public IOpenAiToolResourcesAssistant<IOpenAiAssistant> WithToolResources()
        {
            Request.ToolResources ??= new();
            return new OpenAiToolResourcesAssistant<IOpenAiAssistant>(this, Request.ToolResources);
        }

        public ValueTask<AssistantRequest> CreateAsync(CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, _version, null, string.Empty, null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                        Logger,
                        cancellationToken);
        }

        public ValueTask<DeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                DeleteAsync<DeleteResponse>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, _version, null, $"/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                         Logger,
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
                        OpenAiType.Assistant, _version, null, string.Empty, querystring),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                         Logger,
                        cancellationToken);
        }
        public ValueTask<AssistantRequest> RetrieveAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                GetAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, _version, null, $"/{id}", null),
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                         Logger,
                        cancellationToken);
        }

        public ValueTask<AssistantRequest> UpdateAsync(string id, CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper.
                PostAsync<AssistantRequest>(
                    DefaultServices.Configuration.GetUri(
                        OpenAiType.Assistant, _version, null, $"/{id}", null),
                        Request,
                        BetaRequest.OpenAiBetaHeaders,
                        DefaultServices.Configuration,
                         Logger,
                        cancellationToken);
        }
    }
}
