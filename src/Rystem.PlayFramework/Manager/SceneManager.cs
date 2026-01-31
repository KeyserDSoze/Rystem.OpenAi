using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class SceneManager : ISceneManager
    {
        private readonly HttpContext? _httpContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFactory<IOpenAi> _openAiFactory;
        private readonly IFactory<IScene> _sceneFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PlayHandler _playHandler;
        private readonly FunctionsHandler _functionsHandler;
        private readonly IResponseParser _responseParser;
        private readonly ICacheService? _cacheService;
        private readonly SceneManagerSettings? _settings;
        private readonly IPlanner? _planner;
        private readonly ISummarizer? _summarizer;

        public SceneManager(IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IFactory<IOpenAi> openAiFactory,
            IFactory<IScene> sceneFactory,
            IHttpClientFactory httpClientFactory,
            PlayHandler playHandler,
            FunctionsHandler functionsHandler,
            IResponseParser responseParser,
            ICacheService? cacheService = null,
            SceneManagerSettings? settings = null,
            IPlanner? planner = null,
            ISummarizer? summarizer = null)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _serviceProvider = serviceProvider;
            _openAiFactory = openAiFactory;
            _sceneFactory = sceneFactory;
            _httpClientFactory = httpClientFactory;
            _playHandler = playHandler;
            _functionsHandler = functionsHandler;
            _responseParser = responseParser;
            _cacheService = cacheService;
            _settings = settings;
            _planner = planner;
            _summarizer = summarizer;
        }
        /// <summary>
        /// Initialize context with cache data and create the initial chat client.
        /// This is called once at the beginning of ExecuteAsync.
        /// </summary>
        private async ValueTask<AiSceneResponse?> InitializeContextAsync(string startingMessage, SceneRequestSettings requestSettings, CancellationToken cancellationToken)
        {
            if (requestSettings.Key == null)
            {
                requestSettings.Key = Guid.NewGuid().ToString();
                requestSettings.KeyHasStartedAsNull = true;
            }
            AiSceneResponse? summarizationResponse = null;
            // Create initial chat client
            var chatClient = _openAiFactory.Create(_settings?.OpenAi.Name)?.Chat;
            if (chatClient == null)
            {
                throw new InvalidOperationException("OpenAI Chat client is not configured.");
            }
            requestSettings.Context = new SceneContext
            {
                ServiceProvider = _serviceProvider,
                InputMessage = startingMessage,
                Properties = requestSettings.Properties ?? [],
                Responses = []
            };

            // Load cache and add to chat client if available
            if (requestSettings is { Key: not null, KeyHasStartedAsNull: false, CacheIsAvoidable: false } && _cacheService != null)
            {
                var oldValues = await _cacheService.GetAsync(requestSettings.Key, cancellationToken);
                if (oldValues != null && oldValues.Count > 0)
                {
                    if (_summarizer != null && _summarizer.ShouldSummarize(oldValues))
                    {
                        var summary = await _summarizer.SummarizeAsync(oldValues, cancellationToken);
                        requestSettings.Context!.ConversationSummary = summary;
                        summarizationResponse = new AiSceneResponse
                        {
                            RequestKey = requestSettings.Key!,
                            Name = "Summarization",
                            Message = summary,
                            ResponseTime = DateTime.UtcNow,
                            Status = AiResponseStatus.Summarizing,
                            Cost = null,
                            TotalCost = 0
                        };
                        chatClient.AddSystemMessage(summary);
                    }
                    else
                    {
                        // Build the old context as before
                        StringBuilder oldRequests = new();
                        oldRequests.AppendLine($"Between triple backtips you can find information that you can use to answer the request.");
                        oldRequests.AppendLine();
                        oldRequests.AppendLine("```");
                        var counter = 1;
                        foreach (var oldValueItem in oldValues)
                        {
                            oldRequests.AppendLine($"{counter}) - type of message: {oldValueItem.Status}");
                            counter++;
                            if (oldValueItem.Message != null)
                            {
                                oldRequests.AppendLine($"- Message: {oldValueItem.Message}");
                            }
                            if (oldValueItem.Name != ScenesBuilder.Request)
                            {
                                oldRequests.AppendLine($"- Called scene: {oldValueItem.Name}");
                            }
                            if (oldValueItem.FunctionName != null)
                            {
                                oldRequests.AppendLine($"- Called function: {oldValueItem.FunctionName}");
                            }
                            if (oldValueItem.Arguments != null)
                            {
                                oldRequests.AppendLine($"- Arguments for function: {oldValueItem.Arguments.ToJson()}");
                            }
                            if (oldValueItem.Response != null)
                            {
                                oldRequests.AppendLine($"- Response: {oldValueItem.Response}");
                            }
                            oldRequests.AppendLine();
                            oldRequests.AppendLine();
                        }
                        oldRequests.AppendLine("```");
                        summarizationResponse = new AiSceneResponse
                        {
                            RequestKey = requestSettings.Key!,
                            Name = "Summarization",
                            Message = oldRequests.ToString(),
                            ResponseTime = DateTime.UtcNow,
                            Status = AiResponseStatus.Summarizing,
                            Cost = null,
                            TotalCost = 0
                        };
                        requestSettings.Context.ConversationSummary = oldRequests.ToString();
                        chatClient.AddSystemMessage(oldRequests.ToString());
                    }
                }
            }
            // Set the chat client in context for centralized access
            requestSettings.Context.ChatClient = chatClient;
            return summarizationResponse;
        }
        /// <summary>
        /// Helper to yield and add response to context in one operation
        /// Ensures all responses are tracked for caching and context continuity
        /// </summary>
        private AiSceneResponse YieldAndTrack(SceneContext context, AiSceneResponse response)
        {
            context.Responses.Add(response);
            return response;
        }

        /// <summary>
        /// Checks if context needs summarization and creates/stores summary if needed.
        /// This is called before each OpenAI ExecuteAsync to optimize token usage for the NEXT request.
        /// NOTE: The summary is stored in cache for the next request - current execution continues with full context
        /// because we cannot remove messages from the existing chatClient.
        /// </summary>
        private async ValueTask<AiSceneResponse?> EnsureSummarizedForNextRequestAsync(
            SceneContext context,
            SceneRequestSettings requestSettings,
            CancellationToken cancellationToken)
        {
            // Check if summarization is needed
            if (_summarizer == null || !_summarizer.ShouldSummarize(context.Responses))
            {
                return null;
            }

            // Summarize current context
            var summary = await _summarizer.SummarizeAsync(context.Responses, cancellationToken);

            context.ConversationSummary = summary;
            context.Responses.Clear();

            // Clear messages but keep tools and settings
            context.ChatClient.ClearMessages();
            context.ChatClient.AddSystemMessage($"The previous conversation has been summarized to optimize token usage. Use the summary below to continue the conversation effectively. The request from the user is: ```{context.InputMessage}```");
            context.ChatClient.AddSystemMessage(summary);

            var response = new AiSceneResponse
            {
                RequestKey = requestSettings.Key!,
                Name = "Summarization",
                Message = summary,
                ResponseTime = DateTime.UtcNow,
                Status = AiResponseStatus.Summarizing,
                Cost = null,
                TotalCost = 0
            };
            return response;
        }

        public async IAsyncEnumerable<AiSceneResponse> ExecuteAsync(string message, Action<SceneRequestSettings>? settings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var requestSettings = new SceneRequestSettings();
            settings?.Invoke(requestSettings);

            // Initialize context with cache data and create initial chat client
            await InitializeContextAsync(message, requestSettings, cancellationToken);

            if (requestSettings.Context?.ConversationSummary != null)
            {
                yield return YieldAndTrack(requestSettings.Context, new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Name = "Summarization",
                    Message = requestSettings.Context?.ConversationSummary,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Summarizing,
                    Cost = null,
                    TotalCost = 0
                });
            }
            else
            {
                requestSettings.Context!.Responses.Add(new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Name = ScenesBuilder.Request,
                    Message = message,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Request,
                    Cost = null,
                    TotalCost = 0, // Initialize total cost
                    Model = requestSettings.Context.ChatClient.ModelName
                });
            }
            requestSettings.Context!.CreateNewDefaultChatClient = () => _openAiFactory.Create(_settings?.OpenAi.Name)!.Chat;
            var mainActorsThatPlayEveryScene = _serviceProvider.GetKeyedServices<IPlayableActor>(ScenesBuilder.MainActor);
            var mainActors = _serviceProvider.GetKeyedServices<IActor>(ScenesBuilder.MainActor);
            await PlayActorsInScene(requestSettings.Context, mainActorsThatPlayEveryScene, cancellationToken);
            await PlayActorsInScene(requestSettings.Context, mainActors, cancellationToken);

            // Create execution plan if planning is enabled
            if (_settings?.Planning.Enabled == true && _planner != null)
            {
                yield return YieldAndTrack(requestSettings.Context, new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = "Creating execution plan...",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Planning,
                    Cost = null,
                    TotalCost = requestSettings.Context.TotalCost
                });

                var plan = await _planner.CreatePlanAsync(requestSettings.Context, requestSettings, cancellationToken);
                requestSettings.Context.ExecutionPlan = plan;

                if (plan.IsValid && plan.Steps.Any())
                {
                    yield return YieldAndTrack(requestSettings.Context, new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = plan.ToJson(),
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning,
                        Cost = null,
                        TotalCost = requestSettings.Context.TotalCost
                    });

                    // Execute plan-based orchestration
                    await foreach (var response in ExecutePlanAsync(requestSettings.Context, requestSettings, message, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        requestSettings.Context.Responses.Add(response);
                        yield return response;
                    }
                }
                else
                {
                    // Fallback to original behavior
                    requestSettings.Context.ChatClient.AddUserMessage(message);
                    await foreach (var response in RequestAsync(requestSettings.Context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        requestSettings.Context.Responses.Add(response);
                        yield return response;
                    }
                }
            }
            else
            {
                // Original behavior without planning
                requestSettings.Context.ChatClient.AddUserMessage(message);
                await foreach (var response in RequestAsync(requestSettings.Context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                {
                    requestSettings.Context.Responses.Add(response);
                    yield return response;
                }
            }

            if (!requestSettings.CacheIsAvoidable && _cacheService != null)
            {
                await _cacheService.SetAsync(requestSettings.Key!, requestSettings.Context.Responses, cancellationToken: cancellationToken);
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> RequestAsync(SceneContext context, SceneRequestSettings requestSettings, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var scenes = _playHandler.ScenesChooser(requestSettings.ScenesToAvoid).ToList();
            foreach (var function in scenes)
            {
                function.Invoke(context.ChatClient);
            }

            // Check if summarization is needed before OpenAI call
            var summarizationResponse = await EnsureSummarizedForNextRequestAsync(context, requestSettings, cancellationToken);
            if (summarizationResponse != null)
                yield return YieldAndTrack(context, summarizationResponse);

            var response = await context.ChatClient.ExecuteAsync(cancellationToken);
            // Calculate cost for initial scene selection request
            var initialCost = context.ChatClient.CalculateCost();
            if (initialCost > 0)
            {
                context.AddCost(initialCost);
            }
            if (response?.Choices?[0].Message?.ToolCalls?.Count > 0)
            {
                foreach (var toolCall in response.Choices[0].Message!.ToolCalls!)
                {
                    var sceneResponse = new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Name = toolCall.Function!.Name,
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.SceneRequest,
                        Cost = initialCost > 0 ? initialCost : null,
                        TotalCost = context.TotalCost,
                        Model = context.ChatClient.ModelName
                    };
                    if (response.Usage != null)
                    {
                        PopulateTokenCounts(sceneResponse, response.Usage);
                    }
                    yield return sceneResponse;
                    var scene = _sceneFactory.Create(toolCall.Function!.Name);
                    if (scene != null)
                    {
                        await foreach (var sceneResponse2 in GetResponseFromSceneAsync(scene, context.InputMessage, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                        {
                            yield return sceneResponse2;
                        }
                    }
                }
                if (scenes.Count > 0)
                {
                    var director = _serviceProvider.GetService<IDirector>();
                    if (director != null)
                    {
                        var directorResponse = await director.DirectAsync(context, requestSettings, cancellationToken);
                        if (directorResponse.ExecuteAgain)
                        {
                            requestSettings.AvoidScenes(directorResponse.CutScenes ?? []);
                            context.ChatClient.ClearTools();
                            await foreach (var furtherResponse in RequestAsync(context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                            {
                                yield return furtherResponse;
                            }
                        }
                        else
                        {
                            yield return new AiSceneResponse
                            {
                                RequestKey = requestSettings.Key!,
                                Status = AiResponseStatus.FinishedOk,
                                ResponseTime = DateTime.UtcNow,
                                Message = context.TotalCost > 0 ? $"Completed. Total cost: {context.TotalCost:F6}" : null,
                                Cost = null,
                                TotalCost = context.TotalCost,
                                Model = context.ChatClient.ModelName
                            };
                        }
                    }
                }
            }
            else
            {
                var finishedResponse = new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = response?.Choices?[0].Message?.Content,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.FinishedNoTool,
                    Cost = initialCost > 0 ? initialCost : null,
                    TotalCost = context.TotalCost,
                    Model = context.ChatClient.ModelName
                };
                if (response?.Usage != null)
                {
                    PopulateTokenCounts(finishedResponse, response.Usage);
                }
                yield return finishedResponse;
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> GetResponseFromSceneAsync(IScene scene, string message, SceneContext context, SceneRequestSettings sceneRequestSettings, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            context.ChatClient.ClearTools();
            context.CurrentSceneName = scene.Name;
            var sceneActors = _serviceProvider.GetKeyedServices<IActor>(scene.Name);
            await PlayActorsInScene(context, mainActorsThatPlayEveryScene, cancellationToken);
            await PlayActorsInScene(context, sceneActors, cancellationToken);

            // Add regular functions
            foreach (var function in _functionsHandler.FunctionsChooser(scene.Name))
            {
                function.Invoke(context.ChatClient);
            }

            // Add MCP elements if scene uses MCP server
            if (!string.IsNullOrEmpty(scene.McpServerName))
            {
                var mcpRegistry = _serviceProvider.GetRequiredService<McpRegistry>();
                var client = mcpRegistry.GetClient(scene.McpServerName);

                if (client != null)
                {
                    var filter = scene.McpSceneFilter ?? new McpSceneFilter();

                    // TOOLS
                    if (filter.ToolsEnabled)
                    {
                        var tools = await client.ListToolsAsync(cancellationToken);
                        foreach (var tool in tools.Where(t => filter.MatchesTool(t.Name)))
                        {
                            context.ChatClient.AddFunctionTool(tool);

                            // Register McpToolCall for later execution
                            var functionKey = $"{scene.McpServerName}_{tool.Name}";
                            _functionsHandler[functionKey].McpToolCall = new McpToolCall
                            {
                                ServerName = scene.McpServerName,
                                ToolName = tool.Name,
                                Client = client
                            };
                        }
                    }

                    // RESOURCES - inject as system messages
                    if (filter.ResourcesEnabled)
                    {
                        var resources = await client.ListResourcesAsync(cancellationToken);
                        foreach (var resource in resources.Where(r => filter.MatchesResource(r.Uri)))
                        {
                            var content = await client.ReadResourceAsync(resource.Uri, cancellationToken);
                            if (!string.IsNullOrEmpty(content.Content))
                            {
                                context.ChatClient.AddSystemMessage($"📄 {resource.Name}:\n{content.Content}");
                            }
                        }
                    }

                    // PROMPTS - inject as system messages
                    if (filter.PromptsEnabled)
                    {
                        var prompts = await client.ListPromptsAsync(cancellationToken);
                        foreach (var prompt in prompts.Where(p => filter.MatchesPrompt(p.Name)))
                        {
                            var content = await client.GetPromptAsync(prompt.Name, null, cancellationToken);
                            var promptText = string.Join("\n", content.Content.Select(c => c.Text ?? c.ResourceUri ?? ""));
                            if (!string.IsNullOrEmpty(promptText))
                            {
                                context.ChatClient.AddSystemMessage(promptText);
                            }
                        }
                    }
                }
            }

            context.ChatClient.AddUserMessage(message);

            // Check if summarization is needed before OpenAI call
            var summarizationResponse = await EnsureSummarizedForNextRequestAsync(context, sceneRequestSettings, cancellationToken);
            if (summarizationResponse != null)
                yield return YieldAndTrack(context, summarizationResponse);

            var response = await context.ChatClient.ExecuteAsync(cancellationToken);

            // Calculate cost for this OpenAI request
            var requestCost = context.ChatClient.CalculateCost();
            if (requestCost > 0)
            {
                context.AddCost(requestCost);
            }

            await foreach (var result in GetResponseAsync(scene.Name, scene.HttpClientName, context, sceneRequestSettings, response, requestCost, cancellationToken))
            {
                yield return result;
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> GetResponseAsync(string sceneName, string? clientName, SceneContext context, SceneRequestSettings sceneRequestSettings, ChatResult chatResponse, decimal lastRequestCost, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (chatResponse.Choices?[0].Message?.ToolCalls?.Count > 0)
            {
                foreach (var toolCall in chatResponse.Choices![0].Message!.ToolCalls!)
                {
                    var functionName = toolCall.Function!.Name!;
                    var arguments = toolCall.Function!.Arguments!;

                    // Check if this tool was already executed in this scene
                    if (context.HasExecutedTool(sceneName, functionName, arguments))
                    {
                        var skippedResponse = new AiSceneResponse
                        {
                            RequestKey = sceneRequestSettings.Key!,
                            Name = sceneName,
                            FunctionName = functionName,
                            Message = $"Tool '{functionName}' already executed in scene '{sceneName}', skipping duplicate execution",
                            ResponseTime = DateTime.UtcNow,
                            Status = AiResponseStatus.ToolSkipped,
                            Cost = null, // No new cost for skipped tools
                            TotalCost = context.TotalCost,
                            Model = context.ChatClient.ModelName
                        };
                        if (chatResponse.Usage != null)
                        {
                            PopulateTokenCounts(skippedResponse, chatResponse.Usage);
                        }
                        yield return skippedResponse;
                        continue;
                    }

                    var responseAsJson = string.Empty;
                    var function = _functionsHandler[functionName];
                    if (function.HasMcpToolCall)
                    {
                        responseAsJson = await ExecuteMcpToolAsync(function.McpToolCall!, arguments, cancellationToken);
                    }
                    else if (function.HasHttpRequest)
                    {
                        responseAsJson = await ExecuteHttpClientAsync(clientName, function.HttpRequest!, arguments, cancellationToken);
                    }
                    else if (function.HasService)
                    {
                        responseAsJson = await ExecuteServiceAsync(function.Service!, context, arguments, cancellationToken);
                    }

                    // Mark tool as executed
                    context.MarkToolExecuted(sceneName, functionName, arguments);

                    var toolResponse = new AiSceneResponse
                    {
                        RequestKey = sceneRequestSettings.Key!,
                        Name = sceneName,
                        FunctionName = functionName,
                        Arguments = arguments,
                        Response = responseAsJson,
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.FunctionRequest,
                        Cost = lastRequestCost > 0 ? lastRequestCost : null, // Cost from the last chat request
                        TotalCost = context.TotalCost,
                        Model = context.ChatClient.ModelName
                    };
                    if (chatResponse.Usage != null)
                    {
                        PopulateTokenCounts(toolResponse, chatResponse.Usage);
                    }
                    yield return toolResponse;
                    context.ChatClient.AddSystemMessage($"Response for function {functionName}: {responseAsJson}");
                }
            }

            // Check if summarization is needed before OpenAI call
            var summarizationResponse = await EnsureSummarizedForNextRequestAsync(context, sceneRequestSettings, cancellationToken);
            if (summarizationResponse != null)
                yield return YieldAndTrack(context, summarizationResponse);

            chatResponse = await context.ChatClient.ExecuteAsync(cancellationToken);

            // Calculate cost for the next request
            var nextRequestCost = context.ChatClient.CalculateCost();
            if (nextRequestCost > 0)
            {
                context.AddCost(nextRequestCost);
            }

            if (chatResponse.Choices?[0].Message?.ToolCalls?.Count > 0)
            {
                await foreach (var result in GetResponseAsync(sceneName, clientName, context, sceneRequestSettings, chatResponse, nextRequestCost, cancellationToken))
                {
                    yield return result;
                }
            }
            else
            {
                var runningResponse = new AiSceneResponse
                {
                    RequestKey = sceneRequestSettings.Key!,
                    Name = sceneName,
                    Message = chatResponse.Choices?[0].Message?.Content,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Running,
                    Cost = nextRequestCost > 0 ? nextRequestCost : null,
                    TotalCost = context.TotalCost,
                    Model = context.ChatClient.ModelName
                };
                if (chatResponse.Usage != null)
                {
                    PopulateTokenCounts(runningResponse, chatResponse.Usage);
                }
                yield return runningResponse;
            }
        }
        private async Task<string?> ExecuteServiceAsync(ServiceHandler serviceHandler, SceneContext sceneContext, string argumentAsJson, CancellationToken cancellationToken)
        {
            try
            {
                var json = ParseJson(argumentAsJson);
                sceneContext.Jsons.Add(json);
                var serviceBringer = new ServiceBringer { Parameters = [] };
                foreach (var input in serviceHandler.Actions)
                {
                    await input.Value(json, serviceBringer);
                }
                var response = await serviceHandler.Call(_serviceProvider, serviceBringer, sceneContext, cancellationToken);
                if (response == null)
                    return string.Empty;
                if (response.IsT0)
                {
                    return _responseParser.ParseResponse(response.CastT0);
                }

                return response.CastT1.Message;
            }
            catch (JsonException ex)
            {
                // Return error message that LLM can understand and correct
                return $"{{\"error\": \"JSON deserialization failed: {ex.Message}. Please check the parameter types and values. Arguments received: {argumentAsJson}\"}}";
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Failed to deserialize"))
            {
                // Return error from SceneBuilder parameter deserialization
                return $"{{\"error\": \"{ex.Message}\"}}";
            }
            catch (Exception ex)
            {
                // Catch any other unexpected error
                return $"{{\"error\": \"Service execution failed: {ex.Message}\"}}";
            }
        }

        private async Task<string?> ExecuteMcpToolAsync(McpToolCall mcpCall, string argumentsJson, CancellationToken cancellationToken)
        {
            try
            {
                var executor = _serviceProvider.GetRequiredService<IMcpExecutor>();
                return await executor.ExecuteToolAsync(mcpCall, argumentsJson, cancellationToken);
            }
            catch (Exception ex)
            {
                return $"{{\"error\": \"MCP tool execution failed: {ex.Message}\"}}";
            }
        }

        private async Task<string?> ExecuteHttpClientAsync(string? clientName, HttpHandler httpHandler, string argumentAsJson, CancellationToken cancellationToken)
        {
            var json = ParseJson(argumentAsJson);
            var httpBringer = new HttpBringer();
            using var httpClient = clientName == null ? _httpClientFactory.CreateClient() : _httpClientFactory.CreateClient(clientName);
            await httpHandler.Call(httpBringer);
            foreach (var actions in httpHandler.Actions)
            {
                await actions.Value(json, httpBringer);
            }
            var message = new HttpRequestMessage
            {
                Content = httpBringer.BodyAsJson != null ? new StringContent(httpBringer.BodyAsJson, Encoding.UTF8, "application/json") : null,
                Headers = { { "Accept", "application/json" } },
                RequestUri = new Uri($"{httpClient.BaseAddress}{httpBringer.RewrittenUri ?? httpHandler.Uri}{(httpBringer.Query != null ? (httpHandler.Uri.Contains('?') ? $"&{httpBringer.Query}" : $"?{httpBringer.Query}") : string.Empty)}"),
                Method = new HttpMethod(httpBringer.Method!)
            };
            var authorization = _httpContext?.Request.Headers.Authorization.ToString();
            if (authorization != null)
            {
                var bearer = authorization.Split(' ');
                if (bearer.Length > 1)
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(bearer[0], bearer[1]);
            }
            var request = await httpClient.SendAsync(message, cancellationToken);
            var responseString = await request.Content.ReadAsStringAsync(cancellationToken);
            return responseString;
        }
        private static Dictionary<string, string> ParseJson(string json)
        {
            var result = new Dictionary<string, string>();
            using var document = JsonDocument.Parse(json);
            foreach (var element in document.RootElement.EnumerateObject())
            {
                if (element.Value.ValueKind == JsonValueKind.Object || element.Value.ValueKind == JsonValueKind.Array)
                {
                    result.Add(element.Name, element.Value.GetRawText());
                }
                else
                {
                    result.Add(element.Name, element.Value.ToString());
                }
            }

            return result;
        }
        private static async ValueTask PlayActorsInScene(SceneContext context, IEnumerable<IPlayableActor>? actors, CancellationToken cancellationToken)
        {
            if (actors != null)
            {
                foreach (var actor in actors)
                {
                    var systemMessage = await actor.PlayAsync(context, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(systemMessage.Message))
                        context.ChatClient.AddSystemMessage(systemMessage.Message);
                }
            }
        }

        /// <summary>
        /// Helper to populate token counts in AiSceneResponse from ChatUsage
        /// </summary>
        private static void PopulateTokenCounts(AiSceneResponse response, ChatUsage usage)
        {
            response.InputTokens = usage.PromptTokens;
            response.CachedInputTokens = usage.PromptTokensDetails?.CachedTokens ?? 0;
            response.OutputTokens = usage.CompletionTokens;
            response.TotalTokens = usage.TotalTokens;
        }

        /// <summary>
        /// Helper to calculate aggregated token statistics from all responses
        /// </summary>
        private static (int InputTokens, int CachedInputTokens, int OutputTokens, int TotalTokens)
            CalculateAggregatedTokens(List<AiSceneResponse> responses)
        {
            var totalInput = 0;
            var totalCached = 0;
            var totalOutput = 0;
            var totalTokens = 0;

            foreach (var response in responses)
            {
                if (response.InputTokens.HasValue)
                    totalInput += response.InputTokens.Value;
                if (response.CachedInputTokens.HasValue)
                    totalCached += response.CachedInputTokens.Value;
                if (response.OutputTokens.HasValue)
                    totalOutput += response.OutputTokens.Value;
                if (response.TotalTokens.HasValue)
                    totalTokens += response.TotalTokens.Value;
            }

            return (totalInput, totalCached, totalOutput, totalTokens);
        }
        private async IAsyncEnumerable<AiSceneResponse> ExecutePlanAsync(SceneContext context, SceneRequestSettings requestSettings, string message, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (context.ExecutionPlan == null || !context.ExecutionPlan.Steps.Any())
            {
                yield break;
            }

            // Execute each step in the plan
            foreach (var step in context.ExecutionPlan.Steps.OrderBy(s => s.Order))
            {
                if (step.IsCompleted)
                    continue;

                yield return YieldAndTrack(context, new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Name = step.SceneName,
                    Message = $"Executing step {step.Order}: {step.Purpose}",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.SceneRequest,
                    Cost = null,
                    TotalCost = context.TotalCost
                });

                var scene = _sceneFactory.Create(step.SceneName);
                if (scene != null)
                {
                    await foreach (var sceneResponse in GetResponseFromSceneAsync(scene, message, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        // Don't add here - already added by GetResponseFromSceneAsync
                        yield return sceneResponse;
                    }
                    step.IsCompleted = true;
                }
            }

            // After executing all planned steps, check if we should continue
            if (_planner is DeterministicPlanner deterministicPlanner)
            {
                yield return YieldAndTrack(context, new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = "Checking if more execution is needed...",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Planning,
                    Cost = null,
                    TotalCost = context.TotalCost
                });

                var continuationCheck = await deterministicPlanner.ShouldContinueExecutionAsync(context, requestSettings, cancellationToken);

                if (continuationCheck is { ShouldContinue: true, CanAnswerNow: false })
                {
                    yield return YieldAndTrack(context, new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = $"Continuing execution: {continuationCheck.Reasoning}",
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning,
                        Cost = null,
                        TotalCost = context.TotalCost
                    });

                    // Create a new plan for missing information
                    var newPlan = await _planner.CreatePlanAsync(context, requestSettings, cancellationToken);
                    if (newPlan.IsValid && newPlan.Steps.Any())
                    {
                        context.ExecutionPlan = newPlan;
                        await foreach (var response in ExecutePlanAsync(context, requestSettings, message, mainActorsThatPlayEveryScene, cancellationToken))
                        {
                            // Don't add here - already added by recursive ExecutePlanAsync
                            yield return response;
                        }
                    }
                    else
                    {
                        // No new plan but should continue - generate final response
                        await foreach (var response in GenerateFinalResponseAsync(context, requestSettings, message, cancellationToken))
                        {
                            // Don't add here - already added by GenerateFinalResponseAsync
                            yield return response;
                        }
                    }
                }
                else
                {
                    // Can answer now - generate final response based on gathered information
                    yield return YieldAndTrack(context, new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = continuationCheck.Reasoning,
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning,
                        Cost = null,
                        TotalCost = context.TotalCost
                    });

                    await foreach (var response in GenerateFinalResponseAsync(context, requestSettings, message, cancellationToken))
                    {
                        // Don't add here - already added by GenerateFinalResponseAsync
                        yield return response;
                    }
                }
            }
            else
            {
                // Fallback to director if not using deterministic planner
                var director = _serviceProvider.GetService<IDirector>();
                if (director != null)
                {
                    var directorResponse = await director.DirectAsync(context, requestSettings, cancellationToken);
                    if (directorResponse.ExecuteAgain)
                    {
                        context.ChatClient.ClearTools();
                        requestSettings.AvoidScenes(directorResponse.CutScenes ?? []);

                        var scenes = _playHandler.ScenesChooser(requestSettings.ScenesToAvoid).ToList();
                        foreach (var function in scenes)
                        {
                            function.Invoke(context.ChatClient);
                        }

                        // Check if summarization is needed before OpenAI call
                        var summarizationResponse = await EnsureSummarizedForNextRequestAsync(context, requestSettings, cancellationToken);
                        if (summarizationResponse != null)
                            yield return YieldAndTrack(context, summarizationResponse);

                        var response = await context.ChatClient.ExecuteAsync(cancellationToken);

                        // Calculate cost for director request
                        var directorCost = context.ChatClient.CalculateCost();
                        if (directorCost > 0)
                        {
                            context.AddCost(directorCost);
                        }

                        if (response.Choices?[0].Message?.ToolCalls?.Count > 0)
                        {
                            foreach (var toolCall in response.Choices[0].Message!.ToolCalls!)
                            {
                                yield return YieldAndTrack(context, new AiSceneResponse
                                {
                                    RequestKey = requestSettings.Key!,
                                    Name = toolCall.Function!.Name,
                                    ResponseTime = DateTime.UtcNow,
                                    Status = AiResponseStatus.SceneRequest,
                                    Cost = directorCost > 0 ? directorCost : null,
                                    TotalCost = context.TotalCost,
                                    Model = context.ChatClient.ModelName
                                });

                                var scene = _sceneFactory.Create(toolCall.Function!.Name);
                                if (scene != null)
                                {
                                    await foreach (var sceneResponse in GetResponseFromSceneAsync(scene, context.InputMessage, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                                    {
                                        // Don't add here - already added by GetResponseFromSceneAsync
                                        yield return sceneResponse;
                                    }
                                }
                            }
                        }
                    }
                }

                var (inputTokens, cachedTokens, outputTokens, totalTokens) = CalculateAggregatedTokens(context.Responses);
                var tokenSummary = $"Tokens: {inputTokens} input + {cachedTokens} cached (10% cost) + {outputTokens} output = {totalTokens} total";

                yield return YieldAndTrack(context, new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Status = AiResponseStatus.FinishedOk,
                    ResponseTime = DateTime.UtcNow,
                    Message = context.TotalCost > 0 ?
                        $"Completed. Total cost: ${context.TotalCost:F6}. {tokenSummary}" :
                        $"Completed. {tokenSummary}",
                    Cost = null,
                    TotalCost = context.TotalCost,
                    Model = context.ChatClient.ModelName
                });
            }
        }

        /// <summary>
        /// Generate final response to user based on all gathered information
        /// </summary>
        private async IAsyncEnumerable<AiSceneResponse> GenerateFinalResponseAsync(
            SceneContext context,
            SceneRequestSettings requestSettings,
            string originalMessage,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Create a fresh chat client for final response
            var finalChatClient = context.CreateNewDefaultChatClient!();

            // Temporarily store the original chatClient and use the new one
            var originalChatClient = context.ChatClient;
            context.ChatClient = finalChatClient;

            // Add main actors
            var mainActorsThatPlayEveryScene = _serviceProvider.GetKeyedServices<IPlayableActor>(ScenesBuilder.MainActor);
            await PlayActorsInScene(context, mainActorsThatPlayEveryScene, cancellationToken);

            // Build context from all executed scenes and tools
            var executionSummary = new StringBuilder();
            executionSummary.AppendLine("You have completed gathering information for the user's request.");
            executionSummary.AppendLine();

            foreach (var executedScene in context.ExecutedScenes)
            {
                executionSummary.AppendLine($"Scene: {executedScene.Key}");
                if (executedScene.Value.Any())
                {
                    executionSummary.AppendLine($"  Tools used: {string.Join(", ", executedScene.Value.Select(x => $"{x.ToolName} with arguments: {x.Arguments}"))}");
                }

                // Get responses from this scene
                var sceneResponses = context.Responses
                    .Where(r => r.Name == executedScene.Key)
                    .ToList();

                foreach (var response in sceneResponses)
                {
                    if (response is { FunctionName: not null, Response: not null })
                    {
                        executionSummary.AppendLine($"  - {response.FunctionName}: {response.Response}");
                    }
                    if (response is { Message: not null, Status: AiResponseStatus.Running })
                    {
                        executionSummary.AppendLine($"  - Result: {response.Message}");
                    }
                }
                executionSummary.AppendLine();
            }

            context.ChatClient.AddSystemMessage($@"You have completed gathering information for the user's request.

{executionSummary}

Now, provide a complete and coherent answer to the user's original question using ALL the information gathered above.
Be concise but complete. Do not mention the tools or scenes you used - just answer the question naturally.");

            context.ChatClient.AddUserMessage(originalMessage);

            var finalResponse = await context.ChatClient.ExecuteAsync(cancellationToken);

            // Calculate cost for final response
            var finalCost = context.ChatClient.CalculateCost();
            if (finalCost > 0)
            {
                context.AddCost(finalCost);
            }

            var finalMessage = finalResponse.Choices?[0].Message?.Content;

            var finalSceneResponse = new AiSceneResponse
            {
                RequestKey = requestSettings.Key!,
                Message = finalMessage,
                ResponseTime = DateTime.UtcNow,
                Status = AiResponseStatus.FinishedOk,
                Cost = finalCost > 0 ? finalCost : null,
                TotalCost = context.TotalCost,
                Model = context.ChatClient.ModelName
            };
            if (finalResponse.Usage != null)
            {
                PopulateTokenCounts(finalSceneResponse, finalResponse.Usage);
            }

            // Restore original chatClient
            context.ChatClient = originalChatClient;

            yield return YieldAndTrack(context, finalSceneResponse);
        }
    }
}
