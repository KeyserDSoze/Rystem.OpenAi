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
            ICacheService? cacheService = null,
            SceneManagerSettings? settings = null,
            IPlanner? planner = null,
            ISummarizer? summarizer = null)
        {
            _httpContext = httpContextAccessor?.HttpContext;
            _serviceProvider = serviceProvider;
            _openAiFactory = openAiFactory;
            _sceneFactory = sceneFactory;
            _httpClientFactory = httpClientFactory;
            _playHandler = playHandler;
            _functionsHandler = functionsHandler;
            _cacheService = cacheService;
            _settings = settings;
            _planner = planner;
            _summarizer = summarizer;
        }
        private readonly Dictionary<string, string> _cacheValue = [];
        private async ValueTask<IOpenAiChat> GetChatClientAsync(string startingMessage, SceneRequestSettings requestSettings, CancellationToken cancellationToken)
        {
            var chatClient = _openAiFactory.Create(_settings?.OpenAi.Name)?.Chat;
            if (chatClient == null)
            {
                throw new InvalidOperationException("OpenAI Chat client is not configured.");
            }
            List<AiSceneResponse>? oldValue = null;
            if (requestSettings.Key != null && !requestSettings.KeyHasStartedAsNull && !requestSettings.CacheIsAvoidable && _cacheService != null)
            {
                if (!_cacheValue.ContainsKey(requestSettings.Key))
                {
                    oldValue = await _cacheService.GetAsync(requestSettings.Key, cancellationToken);

                    // Check if we need to summarize
                    if (_summarizer != null && oldValue != null && _summarizer.ShouldSummarize(oldValue))
                    {
                        var summary = await _summarizer.SummarizeAsync(oldValue, cancellationToken);
                        _cacheValue.TryAdd(requestSettings.Key, summary);

                        // Store the summary for the context
                        if (requestSettings.Context != null)
                        {
                            requestSettings.Context.ConversationSummary = summary;
                        }
                    }
                    else
                    {
                        // Build the old context as before
                        StringBuilder oldRequests = new();
                        oldRequests.AppendLine($"Between triple backtips you can find information that you can use to answer the request.");
                        oldRequests.AppendLine();
                        oldRequests.AppendLine("```");
                        var counter = 1;
                        foreach (var oldValueItem in oldValue)
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
                        _cacheValue.TryAdd(requestSettings.Key, oldRequests.ToString());
                    }
                }
                chatClient.AddSystemMessage(_cacheValue[requestSettings.Key]);
            }
            if (requestSettings.Context != null)
                requestSettings.Context.InputMessage = startingMessage;
            requestSettings.Context ??= new SceneContext
            {
                ServiceProvider = _serviceProvider,
                InputMessage = startingMessage,
                Properties = requestSettings.Properties ?? [],
                Responses = oldValue?.Select(x => x).ToList() ?? []
            };
            return chatClient;
        }
        public async IAsyncEnumerable<AiSceneResponse> ExecuteAsync(string message, Action<SceneRequestSettings>? settings = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var requestSettings = new SceneRequestSettings();
            settings?.Invoke(requestSettings);
            if (requestSettings.Key == null)
            {
                requestSettings.Key = Guid.NewGuid().ToString();
                requestSettings.KeyHasStartedAsNull = true;
            }
            var chatClient = await GetChatClientAsync(message, requestSettings, cancellationToken);
            requestSettings.Context!.Responses.Add(new AiSceneResponse
            {
                RequestKey = requestSettings.Key!,
                Name = ScenesBuilder.Request,
                Message = message,
                ResponseTime = DateTime.UtcNow,
                Status = AiResponseStatus.Request
            });
            requestSettings.Context.CreateNewDefaultChatClient = () => _openAiFactory.Create(_settings?.OpenAi.Name)!.Chat!;
            var mainActorsThatPlayEveryScene = _serviceProvider.GetKeyedServices<IPlayableActor>(ScenesBuilder.MainActor);
            var mainActors = _serviceProvider.GetKeyedServices<IActor>(ScenesBuilder.MainActor);
            await PlayActorsInScene(requestSettings.Context, chatClient, mainActorsThatPlayEveryScene, cancellationToken);
            await PlayActorsInScene(requestSettings.Context, chatClient, mainActors, cancellationToken);

            // Create execution plan if planning is enabled
            if (_settings?.Planning.Enabled == true && _planner != null)
            {
                yield return new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = "Creating execution plan...",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Planning
                };

                var plan = await _planner.CreatePlanAsync(requestSettings.Context, requestSettings, cancellationToken);
                requestSettings.Context.ExecutionPlan = plan;

                if (plan.IsValid && plan.Steps.Any())
                {
                    yield return new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = $"Plan created with {plan.Steps.Count} steps: {plan.Reasoning}",
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning
                    };

                    // Execute plan-based orchestration
                    await foreach (var response in ExecutePlanAsync(chatClient, requestSettings.Context, requestSettings, message, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        requestSettings.Context.Responses.Add(response);
                        yield return response;
                    }
                }
                else
                {
                    // Fallback to original behavior
                    chatClient.AddUserMessage(message);
                    await foreach (var response in RequestAsync(chatClient, requestSettings.Context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        requestSettings.Context.Responses.Add(response);
                        yield return response;
                    }
                }
            }
            else
            {
                // Original behavior without planning
                chatClient.AddUserMessage(message);
                await foreach (var response in RequestAsync(chatClient, requestSettings.Context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                {
                    requestSettings.Context.Responses.Add(response);
                    yield return response;
                }
            }

            if (!requestSettings.CacheIsAvoidable && _cacheService != null)
            {
                await _cacheService.SetAsync(requestSettings.Key, requestSettings.Context.Responses, cancellationToken: cancellationToken);
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> RequestAsync(IOpenAiChat chatClient, SceneContext context, SceneRequestSettings requestSettings, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var scenes = _playHandler.ScenesChooser(requestSettings.ScenesToAvoid).ToList();
            foreach (var function in scenes)
            {
                function.Invoke(chatClient);
            }
            var response = await chatClient.ExecuteAsync(cancellationToken);
            if (response?.Choices?[0]?.Message?.ToolCalls?.Count > 0)
            {
                foreach (var toolCall in response.Choices[0].Message!.ToolCalls!)
                {
                    yield return new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Name = toolCall.Function!.Name,
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.SceneRequest,
                    };
                    var scene = _sceneFactory.Create(toolCall.Function!.Name);
                    if (scene != null)
                    {
                        await foreach (var sceneResponse in GetResponseFromSceneAsync(chatClient, scene, context.InputMessage, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                        {
                            yield return sceneResponse;
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
                            chatClient!.ClearTools();
                            await foreach (var furtherResponse in RequestAsync(chatClient, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
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
                            };
                        }
                    }
                }
            }
            else
            {
                yield return new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = response?.Choices?[0]?.Message?.Content,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.FinishedNoTool
                };
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> GetResponseFromSceneAsync(IOpenAiChat chatClient, IScene scene, string message, SceneContext context, SceneRequestSettings sceneRequestSettings, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            chatClient.ClearTools();
            context.CurrentSceneName = scene.Name;
            var sceneActors = _serviceProvider.GetKeyedServices<IActor>(scene.Name);
            await PlayActorsInScene(context, chatClient, mainActorsThatPlayEveryScene, cancellationToken);
            await PlayActorsInScene(context, chatClient, sceneActors, cancellationToken);
            foreach (var function in _functionsHandler.FunctionsChooser(scene.Name))
            {
                function.Invoke(chatClient);
            }
            chatClient.AddUserMessage(message);
            var response = await chatClient.ExecuteAsync(cancellationToken);
            await foreach (var result in GetResponseAsync(scene.Name, scene.HttpClientName, context, sceneRequestSettings, chatClient, response, cancellationToken))
            {
                yield return result;
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> GetResponseAsync(string sceneName, string? clientName, SceneContext context, SceneRequestSettings sceneRequestSettings, IOpenAiChat chatClient, ChatResult chatResponse, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (chatResponse?.Choices?[0]?.Message?.ToolCalls?.Count > 0)
            {
                foreach (var toolCall in chatResponse!.Choices![0]!.Message!.ToolCalls!)
                {
                    var arguments = toolCall.Function!.Arguments!;
                    var functionName = toolCall.Function!.Name!;

                    // Check if this tool was already executed in this scene
                    if (context.HasExecutedTool(sceneName, functionName))
                    {
                        yield return new AiSceneResponse
                        {
                            RequestKey = sceneRequestSettings.Key!,
                            Name = sceneName,
                            FunctionName = functionName,
                            Message = "Tool already executed, skipping",
                            ResponseTime = DateTime.UtcNow,
                            Status = AiResponseStatus.Running
                        };
                        continue;
                    }

                    var responseAsJson = string.Empty;
                    var function = _functionsHandler[functionName];
                    if (function.HasHttpRequest)
                    {
                        responseAsJson = await ExecuteHttpClientAsync(clientName, function.HttpRequest!, arguments, cancellationToken);
                    }
                    else if (function.HasService)
                    {
                        responseAsJson = await ExecuteServiceAsync(function.Service!, context, arguments, cancellationToken);
                    }

                    // Mark tool as executed
                    context.MarkToolExecuted(sceneName, functionName);

                    yield return new AiSceneResponse
                    {
                        RequestKey = sceneRequestSettings.Key!,
                        Name = sceneName,
                        FunctionName = functionName,
                        Arguments = arguments,
                        Response = responseAsJson,
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.FunctionRequest
                    };
                    chatClient.AddSystemMessage($"Response for function {functionName}: {responseAsJson}");
                }
            }
            chatResponse = await chatClient.ExecuteAsync(cancellationToken);
            if (chatResponse?.Choices?[0]?.Message?.ToolCalls?.Count > 0)
            {
                await foreach (var result in GetResponseAsync(sceneName, clientName, context, sceneRequestSettings, chatClient, chatResponse, cancellationToken))
                {
                    yield return result;
                }
            }
            else
                yield return new AiSceneResponse
                {
                    RequestKey = sceneRequestSettings.Key!,
                    Name = sceneName,
                    Message = chatResponse?.Choices?[0]?.Message?.Content,
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Running
                };
        }
        private async Task<string?> ExecuteServiceAsync(ServiceHandler serviceHandler, SceneContext sceneContext, string argumentAsJson, CancellationToken cancellationToken)
        {
            var json = ParseJson(argumentAsJson);
            sceneContext.Jsons.Add(json);
            var serviceBringer = new ServiceBringer() { Parameters = [] };
            foreach (var input in serviceHandler.Actions)
            {
                await input.Value(json, serviceBringer);
            }
            var response = await serviceHandler.Call(_serviceProvider, serviceBringer, sceneContext, cancellationToken);
            if (response == null)
                return string.Empty;
            else if (response.IsT0)
            {
                if (response.CastT0.IsPrimitive())
                    return response.CastT0.ToString();
                else
                    return response.CastT0.ToJson();
            }
            else
                return response.CastT1.Message;
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
                Method = new HttpMethod(httpBringer.Method!.ToString()!)
            };
            var authorization = _httpContext?.Request?.Headers?.Authorization.ToString();
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
            using (var document = JsonDocument.Parse(json))
            {
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
            }
            return result;
        }
        private static async ValueTask PlayActorsInScene(SceneContext context, IOpenAiChat chatClient, IEnumerable<IPlayableActor>? actors, CancellationToken cancellationToken)
        {
            if (actors != null)
            {
                foreach (var actor in actors)
                {
                    var systemMessage = await actor.PlayAsync(context, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(systemMessage.Message))
                        chatClient.AddSystemMessage(systemMessage.Message);
                }
            }
        }
        private async IAsyncEnumerable<AiSceneResponse> ExecutePlanAsync(IOpenAiChat chatClient, SceneContext context, SceneRequestSettings requestSettings, string message, IEnumerable<IPlayableActor>? mainActorsThatPlayEveryScene, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (context.ExecutionPlan == null || !context.ExecutionPlan.Steps.Any())
            {
                yield break;
            }

            // NO - don't add user message here, will be added in each scene
            // chatClient.AddUserMessage(message);

            // Execute each step in the plan
            foreach (var step in context.ExecutionPlan.Steps.OrderBy(s => s.Order))
            {
                if (step.IsCompleted)
                    continue;

                // Skip if scene was already executed
                if (context.HasExecutedScene(step.SceneName))
                {
                    yield return new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Name = step.SceneName,
                        Message = $"Step {step.Order} skipped - scene already executed",
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.SceneRequest
                    };
                    step.IsCompleted = true;
                    continue;
                }

                yield return new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Name = step.SceneName,
                    Message = $"Executing step {step.Order}: {step.Purpose}",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.SceneRequest
                };

                var scene = _sceneFactory.Create(step.SceneName);
                if (scene != null)
                {
                    await foreach (var sceneResponse in GetResponseFromSceneAsync(chatClient, scene, message, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                    {
                        yield return sceneResponse;
                    }
                    step.IsCompleted = true;
                }
            }

            // After executing all planned steps, check if we should continue
            if (_planner is DeterministicPlanner deterministicPlanner)
            {
                yield return new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Message = "Checking if more execution is needed...",
                    ResponseTime = DateTime.UtcNow,
                    Status = AiResponseStatus.Planning
                };

                var continuationCheck = await deterministicPlanner.ShouldContinueExecutionAsync(context, requestSettings, cancellationToken);

                if (continuationCheck.ShouldContinue && !continuationCheck.CanAnswerNow)
                {
                    yield return new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = $"Continuing execution: {continuationCheck.Reasoning}",
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning
                    };

                    // Create a new plan for missing information
                    var newPlan = await _planner.CreatePlanAsync(context, requestSettings, cancellationToken);
                    if (newPlan.IsValid && newPlan.Steps.Any())
                    {
                        context.ExecutionPlan = newPlan;
                        await foreach (var response in ExecutePlanAsync(chatClient, context, requestSettings, message, mainActorsThatPlayEveryScene, cancellationToken))
                        {
                            yield return response;
                        }
                    }
                    else
                    {
                        // No new plan but should continue - generate final response
                        await foreach (var response in GenerateFinalResponseAsync(chatClient, context, requestSettings, message, cancellationToken))
                        {
                            yield return response;
                        }
                    }
                }
                else
                {
                    // Can answer now - generate final response based on gathered information
                    yield return new AiSceneResponse
                    {
                        RequestKey = requestSettings.Key!,
                        Message = $"Generating final response: {continuationCheck.Reasoning}",
                        ResponseTime = DateTime.UtcNow,
                        Status = AiResponseStatus.Planning
                    };

                    await foreach (var response in GenerateFinalResponseAsync(chatClient, context, requestSettings, message, cancellationToken))
                    {
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
                        chatClient.ClearTools();
                        requestSettings.AvoidScenes(directorResponse.CutScenes ?? []);

                        var scenes = _playHandler.ScenesChooser(requestSettings.ScenesToAvoid).ToList();
                        foreach (var function in scenes)
                        {
                            function.Invoke(chatClient);
                        }

                        var response = await chatClient.ExecuteAsync(cancellationToken);
                        if (response?.Choices?[0]?.Message?.ToolCalls?.Count > 0)
                        {
                            foreach (var toolCall in response.Choices[0].Message!.ToolCalls!)
                            {
                                yield return new AiSceneResponse
                                {
                                    RequestKey = requestSettings.Key!,
                                    Name = toolCall.Function!.Name,
                                    ResponseTime = DateTime.UtcNow,
                                    Status = AiResponseStatus.SceneRequest,
                                };
                                var scene = _sceneFactory.Create(toolCall.Function!.Name);
                                if (scene != null)
                                {
                                    await foreach (var sceneResponse in GetResponseFromSceneAsync(chatClient, scene, context.InputMessage, context, requestSettings, mainActorsThatPlayEveryScene, cancellationToken))
                                    {
                                        yield return sceneResponse;
                                    }
                                }
                            }
                        }
                    }
                }

                yield return new AiSceneResponse
                {
                    RequestKey = requestSettings.Key!,
                    Status = AiResponseStatus.FinishedOk,
                    Message = "Plan execution completed",
                    ResponseTime = DateTime.UtcNow,
                };
            }
        }

        /// <summary>
        /// Generate final response to user based on all gathered information
        /// </summary>
        private async IAsyncEnumerable<AiSceneResponse> GenerateFinalResponseAsync(
            IOpenAiChat chatClient,
            SceneContext context,
            SceneRequestSettings requestSettings,
            string originalMessage,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Create a fresh chat client for final response
            var finalChatClient = context.CreateNewDefaultChatClient!();

            // Add main actors
            var mainActorsThatPlayEveryScene = _serviceProvider.GetKeyedServices<IPlayableActor>(ScenesBuilder.MainActor);
            await PlayActorsInScene(context, finalChatClient, mainActorsThatPlayEveryScene, cancellationToken);

            // Build context from all executed scenes and tools
            var executionSummary = new StringBuilder();
            executionSummary.AppendLine("You have executed the following steps to gather information:");
            executionSummary.AppendLine();

            foreach (var executedScene in context.ExecutedScenes)
            {
                executionSummary.AppendLine($"Scene: {executedScene.Key}");
                if (executedScene.Value.Any())
                {
                    executionSummary.AppendLine($"  Tools used: {string.Join(", ", executedScene.Value)}");
                }

                // Get responses from this scene
                var sceneResponses = context.Responses
                    .Where(r => r.Name == executedScene.Key)
                    .ToList();

                foreach (var response in sceneResponses)
                {
                    if (response.FunctionName != null && response.Response != null)
                    {
                        executionSummary.AppendLine($"  - {response.FunctionName}: {response.Response}");
                    }
                    if (response.Message != null && response.Status == AiResponseStatus.Running)
                    {
                        executionSummary.AppendLine($"  - Result: {response.Message}");
                    }
                }
                executionSummary.AppendLine();
            }

            finalChatClient.AddSystemMessage($@"You have completed gathering information for the user's request.

{executionSummary}

Now, provide a complete and coherent answer to the user's original question using ALL the information gathered above.
Be concise but complete. Do not mention the tools or scenes you used - just answer the question naturally.");

            finalChatClient.AddUserMessage(originalMessage);

            var finalResponse = await finalChatClient.ExecuteAsync(cancellationToken);
            var finalMessage = finalResponse?.Choices?[0]?.Message?.Content;

            yield return new AiSceneResponse
            {
                RequestKey = requestSettings.Key!,
                Message = finalMessage,
                ResponseTime = DateTime.UtcNow,
                Status = AiResponseStatus.FinishedOk
            };
        }
    }
}
