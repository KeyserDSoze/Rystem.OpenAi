using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi;
using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    /// <summary>
    /// Deterministic planner that uses forced tools to create structured execution plans
    /// </summary>
    internal sealed class DeterministicPlanner : IPlanner
    {
        private readonly IFactory<IOpenAi>? _openAiFactory;
        private readonly PlayHandler _playHandler;
        private readonly IFactory<IScene> _sceneFactory;
        private readonly FunctionsHandler _functionsHandler;
        private readonly SceneManagerSettings? _settings;

        public DeterministicPlanner(
            IFactory<IOpenAi>? openAiFactory,
            PlayHandler playHandler,
            IFactory<IScene> sceneFactory,
            FunctionsHandler functionsHandler,
            SceneManagerSettings? settings = null)
        {
            _openAiFactory = openAiFactory;
            _playHandler = playHandler;
            _sceneFactory = sceneFactory;
            _functionsHandler = functionsHandler;
            _settings = settings;
        }

        public async Task<ExecutionPlan> CreatePlanAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken)
        {
            if (_openAiFactory == null || context.CreateNewDefaultChatClient == null)
            {
                return CreateEmptyPlan("OpenAI client not available");
            }

            var chatClient = context.CreateNewDefaultChatClient();
            var availableScenes = _playHandler.GetScenes(requestSettings.ScenesToAvoid).ToList();

            if (availableScenes.Count == 0)
            {
                return CreateEmptyPlan("No scenes available");
            }

            // Build available scenes info
            var scenesInfo = availableScenes
                .Select(sceneName =>
                {
                    var scene = _sceneFactory.Create(sceneName);
                    var tools = _functionsHandler.GetFunctionNames(sceneName).ToList();
                    return new AvailableScene
                    {
                        Name = sceneName,
                        Description = scene?.Description ?? sceneName,
                        AvailableTools = tools.Any() ? tools : null
                    };
                })
                .ToList();

            // Create planning tool automatically from type
            var planningTool = CreatePlanningTool();
            chatClient.AddFunctionTool(planningTool);

            // Add system message
            chatClient.AddSystemMessage(@"You are an expert execution planner. Analyze the user's request and create a detailed plan.
IMPORTANT RULES:
1. If the request can be answered without calling any scenes, set needs_execution=false
2. If scenes are needed, set needs_execution=true and provide detailed steps
3. Order steps logically - some steps may depend on previous ones
4. Each step should have a clear purpose
5. List expected tools that will be called in each scene
6. Use step dependencies (depends_on_step) when a step needs data from another

You MUST call the CreateExecutionPlan function to respond.");

            if (context.ConversationSummary != null)
            {
                chatClient.AddSystemMessage($"Previous conversation summary:\n{context.ConversationSummary}");
            }

            // Build planning request
            var planningRequest = new PlanningRequest
            {
                UserRequest = context.InputMessage,
                AvailableScenes = scenesInfo,
                ConversationSummary = context.ConversationSummary
            };

            chatClient.AddUserMessage($"Create an execution plan for this request:\n{JsonSerializer.Serialize(planningRequest, new JsonSerializerOptions { WriteIndented = true })}");

            // Execute and get plan
            var response = await chatClient.ExecuteAsync(cancellationToken);
            var toolCall = response?.Choices?[0]?.Message?.ToolCalls?.FirstOrDefault();

            if (toolCall?.Function?.Arguments == null)
            {
                return CreateEmptyPlan("No planning response received");
            }
            try
            {
                // Use FromJson extension that handles flexible deserialization
                var planningResponse = toolCall.Function.Arguments.FromJson<PlanningResponse>();

                if (planningResponse == null || !planningResponse.NeedsExecution)
                {
                    return new ExecutionPlan
                    {
                        Steps = [],
                        IsValid = false,
                        Reasoning = planningResponse?.Reasoning ?? "No execution needed - can answer directly"
                    };
                }

                var steps = planningResponse.Steps?
                    .Where(s => !string.IsNullOrEmpty(s.SceneName) && !string.IsNullOrEmpty(s.Purpose))
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new PlanStep
                    {
                        SceneName = s.SceneName,
                        Purpose = s.Purpose,
                        ExpectedTools = s.ExpectedTools,
                        Order = s.StepNumber,
                        IsCompleted = false
                    })
                    .ToList() ?? [];

                return new ExecutionPlan
                {
                    Steps = steps,
                    IsValid = steps.Count > 0,
                    Reasoning = planningResponse.Reasoning
                };
            }
            catch (JsonException ex)
            {
                // Log the actual JSON for debugging
                var errorMessage = $"Failed to parse planning response: {ex.Message}\nJSON: {toolCall.Function.Arguments}";
                return CreateEmptyPlan(errorMessage);
            }
        }

        /// <summary>
        /// Check if execution should continue after completing planned steps
        /// </summary>
        public async Task<ContinuationCheckResponse> ShouldContinueExecutionAsync(
            SceneContext context,
            SceneRequestSettings requestSettings,
            CancellationToken cancellationToken)
        {
            if (_openAiFactory == null || context.CreateNewDefaultChatClient == null)
            {
                return new ContinuationCheckResponse
                {
                    ShouldContinue = false,
                    CanAnswerNow = true,
                    Reasoning = "Cannot check continuation without OpenAI client"
                };
            }

            var chatClient = context.CreateNewDefaultChatClient();
            var availableScenes = _playHandler.GetScenes(requestSettings.ScenesToAvoid).ToList();

            // Create continuation check tool automatically from type
            var continuationTool = CreateContinuationCheckTool();
            chatClient.AddFunctionTool(continuationTool);

            chatClient.AddSystemMessage(@"You are checking if more execution is needed or if you can answer the user now.
Analyze what has been executed and decide:
1. Can you answer the user's request with the information gathered? Set can_answer_now=true
2. Do you need to call more scenes/tools? Set should_continue=true
3. If continuing, explain what information is missing

You MUST call the CheckContinuation function to respond.");

            var request = new ContinuationCheckRequest
            {
                UserRequest = context.InputMessage,
                AvailableScenes = availableScenes
            };

            chatClient.AddUserMessage($"Check if execution should continue:\n{JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })}");

            var response = await chatClient.ExecuteAsync(cancellationToken);
            var toolCall = response?.Choices?[0]?.Message?.ToolCalls?.FirstOrDefault();

            if (toolCall?.Function?.Arguments == null)
            {
                return new ContinuationCheckResponse
                {
                    ShouldContinue = false,
                    CanAnswerNow = true,
                    Reasoning = "No response from continuation check"
                };
            }

            try
            {
                // Use FromJson extension that handles flexible deserialization
                return toolCall.Function.Arguments.FromJson<ContinuationCheckResponse>() ?? new ContinuationCheckResponse
                {
                    ShouldContinue = false,
                    CanAnswerNow = true,
                    Reasoning = "Failed to deserialize response"
                };
            }
            catch (JsonException ex)
            {
                var errorMessage = $"Error parsing continuation check: {ex.Message}\nJSON: {toolCall.Function.Arguments}";
                return new ContinuationCheckResponse
                {
                    ShouldContinue = false,
                    CanAnswerNow = true,
                    Reasoning = errorMessage
                };
            }
        }

        private static FunctionTool CreatePlanningTool()
        {
            // Use automatic tool generation from type with all JsonPropertyName and Description attributes
            return typeof(PlanningResponse).ToFunctionTool(
                "CreateExecutionPlan",
                "Creates a structured execution plan with scenes and tools to call. Set needs_execution=false if you can answer directly without calling any scenes. If scenes are needed, provide detailed steps in order.");
        }

        private static FunctionTool CreateContinuationCheckTool()
        {
            // Use automatic tool generation from type with all JsonPropertyName and Description attributes
            return typeof(ContinuationCheckResponse).ToFunctionTool(
                "CheckContinuation",
                "Check if more execution is needed or if you can answer the user now with the information gathered.");
        }

        private static ExecutionPlan CreateEmptyPlan(string reason)
        {
            return new ExecutionPlan
            {
                Steps = [],
                IsValid = false,
                Reasoning = reason
            };
        }
    }
}
