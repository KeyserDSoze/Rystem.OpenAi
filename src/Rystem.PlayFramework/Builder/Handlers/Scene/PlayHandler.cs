using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class PlayHandler
    {
        public SceneHandler this[string sceneName]
        {
            get
            {
                if (!Scenes.ContainsKey(sceneName))
                    Scenes.Add(sceneName, new());
                return Scenes[sceneName];
            }
        }
        public IEnumerable<Action<IOpenAiChat>> ScenesChooser(IEnumerable<string>? toAvoid = null)
        {
            return Scenes.Where(x => x.Value.Chooser != null && (toAvoid == null || !toAvoid.Contains(x.Key))).Select(x => x.Value.Chooser!);
        }
        public IEnumerable<string> GetScenes(IEnumerable<string>? toAvoid = null)
        {
            return Scenes.Where(x => toAvoid == null || !toAvoid.Contains(x.Key)).Select(x => x.Key);
        }
        public IEnumerable<string> ChooseRightPath(string path)
        {
            foreach (var scene in Scenes)
            {
                foreach (var regex in scene.Value.AvailableApiPath)
                {
                    if (regex.IsMatch(path))
                        yield return scene.Key;
                }
            }
        }
        private Dictionary<string, SceneHandler> Scenes { get; } = [];
    }
}
