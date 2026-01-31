using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    public sealed class SceneRequestContext
    {
        public string? ToolName { get; init; }
        public string? Arguments { get; init; }
    }
    public sealed class SceneContext
    {
        public required IServiceProvider ServiceProvider { get; init; }
        public required string InputMessage { get; set; }
        public string? CurrentSceneName { get; set; }
        public List<AiSceneResponse> Responses { get; init; } = [];
        public Func<IOpenAiChat>? CreateNewDefaultChatClient { get; set; }
        public required Dictionary<object, object> Properties { get; init; }
        public List<Dictionary<string, string>> Jsons { get; } = [];

        /// <summary>
        /// The current chat client being used for OpenAI requests.
        /// This is centralized here to avoid passing it as a parameter everywhere.
        /// </summary>
        public IOpenAiChat ChatClient { get; set; } = null!;

        /// <summary>
        /// The current execution plan if planning is enabled.
        /// </summary>
        public ExecutionPlan? ExecutionPlan { get; set; }

        /// <summary>
        /// Summary of previous conversation history.
        /// </summary>
        public string? ConversationSummary { get; set; }

        /// <summary>
        /// Track which scenes have been executed in this request to avoid re-execution.
        /// Key: SceneName, Value: List of tools called in that scene
        /// </summary>
        public Dictionary<string, HashSet<SceneRequestContext>> ExecutedScenes { get; } = [];

        /// <summary>
        /// Track which tools have been executed across all scenes to avoid duplicates.
        /// </summary>
        public HashSet<string> ExecutedTools { get; } = [];

        /// <summary>
        /// Total accumulated cost for all OpenAI requests in this conversation
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Check if a scene has already been executed in this request.
        /// </summary>
        public bool HasExecutedScene(string sceneName) => ExecutedScenes.ContainsKey(sceneName);

        /// <summary>
        /// Check if a specific tool has already been called in a scene.
        /// </summary>
        public bool HasExecutedTool(string sceneName, string toolName, string? arguments) 
            => ExecutedTools.TryGetValue($"{sceneName}.{toolName}.{arguments}", out var _);

        /// <summary>
        /// Mark a tool as executed for a scene.
        /// </summary>
        public void MarkToolExecuted(string sceneName, string toolName, string? arguments)
        {
            if (!ExecutedScenes.ContainsKey(sceneName))
            {
                ExecutedScenes[sceneName] = [];
            }
            ExecutedScenes[sceneName].Add(new SceneRequestContext { ToolName = toolName, Arguments = arguments });
            ExecutedTools.Add($"{sceneName}.{toolName}.{arguments}");
        }
        /// <summary>
        /// Add cost to the total and return the new total
        /// </summary>
        public decimal AddCost(decimal cost)
        {
            TotalCost += cost;
            return TotalCost;
        }

        public T GetProperty<T>(object key)
        {
            if (Properties.TryGetValue(key, out var propertyValue))
            {
                return (T)propertyValue;
            }
            else
            {
                return default!;
            }
        }
        public bool TryGetProperty<T>(object key, out T value)
        {
            if (Properties.TryGetValue(key, out var propertyValue))
            {
                value = (T)propertyValue;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }
        public T GetPropertyOrSetDefault<T>(object key, T? property = default)
        {
            if (TryGetProperty<T>(key, out var dictionaryValue))
            {
                return dictionaryValue;
            }
            else
            {
                if (Properties.ContainsKey(key))
                    Properties[key] = property!;
                else
                    Properties.Add(key, property!);
                return property!;
            }
        }
        public bool TrySetProperty<T>(object key, T value)
        {
            if (Properties.ContainsKey(key))
            {
                return false;
            }
            else
            {
                return Properties.TryAdd(key, value!);
            }
        }
        public bool SetProperty<T>(object key, T value)
        {
            if (Properties.ContainsKey(key))
            {
                Properties[key] = value!;
            }
            else
            {
                return Properties.TryAdd(key, value!);
            }
            return true;
        }
    }
}
