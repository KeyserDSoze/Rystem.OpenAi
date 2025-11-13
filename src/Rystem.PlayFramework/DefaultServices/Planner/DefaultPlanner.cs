using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class DefaultPlanner : IPlanner
    {
        private readonly PlayHandler _playHandler;
        private readonly IFactory<IScene> _sceneFactory;
        private readonly FunctionsHandler _functionsHandler;
        private readonly SceneManagerSettings? _settings;

        public DefaultPlanner(
            PlayHandler playHandler,
            IFactory<IScene> sceneFactory,
            FunctionsHandler functionsHandler,
            SceneManagerSettings? settings = null)
        {
            _playHandler = playHandler;
            _sceneFactory = sceneFactory;
            _functionsHandler = functionsHandler;
            _settings = settings;
        }

        public async Task<ExecutionPlan> CreatePlanAsync(SceneContext context, SceneRequestSettings requestSettings, CancellationToken cancellationToken)
        {
            if (context.CreateNewDefaultChatClient == null)
            {
                return CreateEmptyPlan();
            }

            var chatClient = context.CreateNewDefaultChatClient();
            var availableScenes = _playHandler.GetScenes(requestSettings.ScenesToAvoid).ToList();

            if (availableScenes.Count == 0)
            {
                return CreateEmptyPlan();
            }

            // Build the planning prompt
            var planningPrompt = BuildPlanningPrompt(context, availableScenes);
            chatClient
                .AddSystemMessage("You are an expert planner for multi-agent systems. Analyze the user request and available scenes/tools to create an optimal execution plan.")
                .AddSystemMessage(planningPrompt)
                .AddUserMessage($"Create a detailed execution plan for this request: {context.InputMessage}");

            var response = await chatClient.ExecuteAsync(cancellationToken);
            var planContent = response?.Choices?[0]?.Message?.Content;

            if (string.IsNullOrWhiteSpace(planContent))
            {
                return CreateEmptyPlan();
            }

            // Parse the plan from the AI response
            return ParsePlanFromResponse(planContent, availableScenes);
        }

        private string BuildPlanningPrompt(SceneContext context, List<string> availableScenes)
        {
            var prompt = new StringBuilder();
            prompt.AppendLine("Available scenes and their capabilities:");
            prompt.AppendLine();

            var maxScenes = _settings?.Planning.MaxScenesInPlan ?? 10;
            var scenesToInclude = availableScenes.Take(maxScenes);

            foreach (var sceneName in scenesToInclude)
            {
                var scene = _sceneFactory.Create(sceneName);
                if (scene != null)
                {
                    prompt.AppendLine($"Scene: {scene.Name}");
                    prompt.AppendLine($"Description: {scene.Description}");
                    // Include available tools for this scene
                    var tools = _functionsHandler.FunctionsChooser(scene.Name).ToList();
                    if (tools.Any())
                    {
                        prompt.AppendLine("Available tools:");
                        var functionNames = _functionsHandler.GetFunctionNames(scene.Name);
                        foreach (var functionName in functionNames)
                        {
                            prompt.AppendLine($"  - {functionName}");
                        }
                    }
                    prompt.AppendLine();
                }
            }

            if (context.ConversationSummary != null)
            {
                prompt.AppendLine("Previous conversation summary:");
                prompt.AppendLine(context.ConversationSummary);
                prompt.AppendLine();
            }

            prompt.AppendLine("Create a JSON plan with the following structure:");
            prompt.AppendLine(@"{
  ""steps"": [
    {
      ""sceneName"": ""SceneName"",
      ""purpose"": ""What this step accomplishes"",
      ""order"": 1,
      ""expectedTools"": [""tool1"", ""tool2""]
    }
  ],
  ""reasoning"": ""Explanation of the plan""
}");

            return prompt.ToString();
        }

        private ExecutionPlan ParsePlanFromResponse(string planContent, List<string> availableScenes)
        {
            try
            {
                // Try to extract JSON from markdown code blocks if present
                var jsonContent = ExtractJsonFromMarkdown(planContent);

                var planData = JsonSerializer.Deserialize<PlanData>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (planData?.Steps == null || planData.Steps.Count == 0)
                {
                    return CreateEmptyPlan();
                }

                var steps = planData.Steps
                    .Where(s => !string.IsNullOrWhiteSpace(s.SceneName) && availableScenes.Contains(s.SceneName))
                    .Select((s, index) => new PlanStep
                    {
                        SceneName = s.SceneName!,
                        Purpose = s.Purpose ?? "Execute scene",
                        ExpectedTools = s.ExpectedTools,
                        Order = s.Order > 0 ? s.Order : index + 1,
                        IsCompleted = false
                    })
                    .OrderBy(s => s.Order)
                    .ToList();

                return new ExecutionPlan
                {
                    Steps = steps,
                    IsValid = steps.Count > 0,
                    Reasoning = planData.Reasoning
                };
            }
            catch
            {
                // Fallback to simple parsing or empty plan
                return CreateEmptyPlan();
            }
        }

        private static string ExtractJsonFromMarkdown(string content)
        {
            // Try to find JSON in markdown code blocks
            var jsonStart = content.IndexOf("```json", StringComparison.OrdinalIgnoreCase);
            if (jsonStart >= 0)
            {
                jsonStart += 7; // Length of "```json"
                var jsonEnd = content.IndexOf("```", jsonStart);
                if (jsonEnd > jsonStart)
                {
                    return content.Substring(jsonStart, jsonEnd - jsonStart).Trim();
                }
            }

            // Try to find JSON in any code block
            jsonStart = content.IndexOf("```");
            if (jsonStart >= 0)
            {
                jsonStart += 3;
                var jsonEnd = content.IndexOf("```", jsonStart);
                if (jsonEnd > jsonStart)
                {
                    return content.Substring(jsonStart, jsonEnd - jsonStart).Trim();
                }
            }

            // Assume the entire content is JSON
            return content.Trim();
        }

        private static ExecutionPlan CreateEmptyPlan()
        {
            return new ExecutionPlan
            {
                Steps = [],
                IsValid = false,
                Reasoning = "No valid plan could be created"
            };
        }

        // Helper classes for JSON deserialization
        private class PlanData
        {
            public List<PlanStepData>? Steps { get; set; }
            public string? Reasoning { get; set; }
        }

        private class PlanStepData
        {
            public string? SceneName { get; set; }
            public string? Purpose { get; set; }
            public int Order { get; set; }
            public List<string>? ExpectedTools { get; set; }
        }
    }
}
