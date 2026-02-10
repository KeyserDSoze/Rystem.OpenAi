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
        /// This is called before each OpenAI ExecuteAsync to optimize token usage during runtime.
        /// When threshold is reached, messages are cleared and replaced with a summary,
        /// while preserving all registered tools and settings.
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
            // Play main actors
            await PlayActorsInScene(requestSettings.Context, mainActorsThatPlayEveryScene, cancellationToken);
            await PlayActorsInScene(requestSettings.Context, mainActors.Cast<IPlayableActor>(), cancellationToken);

            // Execute without planning - let LLM choose scenes dynamically
            requestSettings.Context.ChatClient.AddUserMessage(message);
            await foreach (var response in RequestAsync(requestSettings.Context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
            {
                requestSettings.Context.Responses.Add(response);
                yield return response;
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
                            // Director says don't execute again - generate final response to user
                            context.ChatClient.ClearTools();

                            // Make a final call to get textual response for the user
                            var finalSummarizationResponse = await EnsureSummarizedForNextRequestAsync(context, requestSettings, cancellationToken);
                            if (finalSummarizationResponse != null)
                                yield return YieldAndTrack(context, finalSummarizationResponse);

                            var finalResponse = await context.ChatClient.ExecuteAsync(cancellationToken);
                            var finalCost = context.ChatClient.CalculateCost();
                            if (finalCost > 0)
                            {
                                context.AddCost(finalCost);
                            }

                            var finishedResponse = new AiSceneResponse
                            {
                                RequestKey = requestSettings.Key!,
                                Message = finalResponse?.Choices?[0].Message?.Content,
                                ResponseTime = DateTime.UtcNow,
                                Status = AiResponseStatus.FinishedOk,
                                Cost = finalCost > 0 ? finalCost : null,
                                TotalCost = context.TotalCost,
                                Model = context.ChatClient.ModelName
                            };
                            if (finalResponse?.Usage != null)
                            {
                                PopulateTokenCounts(finishedResponse, finalResponse.Usage);
                            }
                            yield return finishedResponse;
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
            var anyToolExecuted = false; // Track if we actually executed any tool

            if (chatResponse.Choices?[0].Message?.ToolCalls?.Count > 0)
            {
                foreach (var toolCall in chatResponse.Choices![0].Message!.ToolCalls!)
                {
                    var functionName = toolCall.Function!.Name!;
                    var arguments = toolCall.Function!.Arguments!;

                    // Check if this tool was already executed in this scene
                    if (context.HasExecutedTool(sceneName, functionName, arguments))
                    {
                        // Get cached response
                        var cachedResponse = context.GetToolResponse(sceneName, functionName, arguments);

                        // Add system message with cached response so LLM knows what was returned
                        context.ChatClient.AddSystemMessage(
                            $"[CACHED] Function '{functionName}' was already called with these arguments. " +
                            $"Previous result: {cachedResponse}");

                        var skippedResponse = new AiSceneResponse
                        {
                            RequestKey = sceneRequestSettings.Key!,
                            Name = sceneName,
                            FunctionName = functionName,
                            Message = $"Tool '{functionName}' already executed in scene '{sceneName}', using cached result",
                            Response = cachedResponse, // Include the cached response
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

                    anyToolExecuted = true; // Mark that we executed at least one tool

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

                    // Mark tool as executed and cache the response
                    context.MarkToolExecuted(sceneName, functionName, arguments, responseAsJson);

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

            // If all tools were skipped (loop detected), generate final response instead of continuing
            if (!anyToolExecuted && chatResponse.Choices?[0].Message?.ToolCalls?.Count > 0)
            {
                // All tools were already executed - don't call OpenAI again, just return a final response
                var finalMessage = chatResponse.Choices?[0].Message?.Content ?? "All requested operations have been completed.";

                var loopBreakResponse = new AiSceneResponse
                {
                    RequestKey = sceneRequestSettings.Key!,
                    Name = sceneName,
                    Message = finalMessage,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Running,
                    Cost = null,
                    TotalCost = context.TotalCost,
                    Model = context.ChatClient.ModelName
                };
                yield return loopBreakResponse;
                yield break; // Exit to prevent infinite loop
            }

            // Only continue with next OpenAI call if we actually executed some tools
            if (anyToolExecuted || chatResponse.Choices?[0].Message?.ToolCalls?.Count == 0)
            {
                // Add all scenes as tools to allow dynamic scene switching
                // This lets the LLM decide to either continue with current scene tools or switch to another scene
                var scenes = _playHandler.ScenesChooser(sceneRequestSettings.ScenesToAvoid).ToList();
                foreach (var sceneFunction in scenes)
                {
                    sceneFunction.Invoke(context.ChatClient);
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
                    // Check if LLM wants to switch to a different scene
                    var firstCall = chatResponse.Choices[0].Message!.ToolCalls![0];
                    var requestedFunction = firstCall.Function!.Name!;

                    // Try to find if this is a scene (not a tool of current scene)
                    var requestedScene = _sceneFactory.Create(requestedFunction);
                    if (requestedScene != null && requestedScene.Name != sceneName)
                    {
                        // LLM wants to switch to a different scene - exit current scene recursion
                        // and let the caller handle the scene switch
                        yield return new AiSceneResponse
                        {
                            RequestKey = sceneRequestSettings.Key!,
                            Name = sceneName,
                            Message = $"Switching from scene '{sceneName}' to scene '{requestedScene.Name}'",
                            ResponseTime = DateTime.UtcNow,
                            Status = AiResponseStatus.SceneRequest,
                            Cost = nextRequestCost > 0 ? nextRequestCost : null,
                            TotalCost = context.TotalCost,
                            Model = context.ChatClient.ModelName
                        };

                        // Execute the new scene
                        await foreach (var sceneResponse in GetResponseFromSceneAsync(requestedScene, context.InputMessage, context, sceneRequestSettings, _serviceProvider.GetKeyedServices<IPlayableActor>(ScenesBuilder.MainActor), cancellationToken))
                        {
                            yield return sceneResponse;
                        }
                        yield break; // Exit current scene
                    }

                    // Otherwise continue with current scene tools (recursive call)
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
    }
}
