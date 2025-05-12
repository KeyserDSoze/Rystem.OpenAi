using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    public sealed class SceneContext
    {
        public required IServiceProvider ServiceProvider { get; init; }
        public required string InputMessage { get; set; }
        public string? CurrentSceneName { get; set; }
        public List<AiSceneResponse> Responses { get; init; } = [];
        public IOpenAiChat? CurrentChatClient { get; set; }
        public Func<IOpenAiChat>? CreateNewDefaultChatClient { get; set; }
        public required Dictionary<object, object> Properties { get; init; }
        public List<Dictionary<string, string>> Jsons { get; } = [];
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
