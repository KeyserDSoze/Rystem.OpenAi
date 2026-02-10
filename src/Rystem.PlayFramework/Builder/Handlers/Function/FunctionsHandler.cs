using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class FunctionsHandler
    {
        public FunctionHandler this[string functionName]
        {
            get
            {
                if (!Functions.ContainsKey(functionName))
                    Functions.Add(functionName, new());
                return Functions[functionName];
            }
        }
        public IEnumerable<Action<IOpenAiChat>> FunctionsChooser(string sceneName)
            => Functions.Where(x => x.Value.Chooser != null && (x.Value.ForEveryScene || x.Value.Scenes.Contains(sceneName))).Select(x => x.Value.Chooser!);

        public IEnumerable<string> GetFunctionNames(string sceneName)
            => Functions.Where(x => x.Value.ForEveryScene || x.Value.Scenes.Contains(sceneName)).Select(x => x.Key);

        /// <summary>
        /// Get function info (name and description) for a specific scene, used by planner
        /// </summary>
        public IEnumerable<(string Name, string? Description)> GetFunctionInfos(string sceneName)
            => Functions
                .Where(x => x.Value.ForEveryScene || x.Value.Scenes.Contains(sceneName))
                .Select(x => (x.Key, x.Value.Description));

        private Dictionary<string, FunctionHandler> Functions { get; } = [];
    }
}
