using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
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
        public Dictionary<string, HashSet<string>> ExecutedScenes { get; } = new();

        /// <summary>
        /// Track which tools have been executed across all scenes to avoid duplicates.
        /// </summary>
        public HashSet<string> ExecutedTools { get; } = new();

        /// <summary>
        /// Check if a scene has already been executed in this request.
        /// </summary>
        public bool HasExecutedScene(string sceneName) => ExecutedScenes.ContainsKey(sceneName);

        /// <summary>
        /// Check if a specific tool has already been called in a scene.
        /// </summary>
        public bool HasExecutedTool(string sceneName, string toolName)
        {
            return ExecutedScenes.TryGetValue(sceneName, out var tools) && tools.Contains(toolName);
        }

        /// <summary>
        /// Mark a tool as executed for a scene.
        /// </summary>
        public void MarkToolExecuted(string sceneName, string toolName)
        {
            if (!ExecutedScenes.ContainsKey(sceneName))
            {
                ExecutedScenes[sceneName] = new HashSet<string>();
            }
            ExecutedScenes[sceneName].Add(toolName);
            ExecutedTools.Add($"{sceneName}.{toolName}");
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
