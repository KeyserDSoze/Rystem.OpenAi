namespace Rystem.PlayFramework
{
    public sealed class SceneRequestSettings
    {
        internal List<string>? ScenesToAvoid { get; set; }
        public SceneRequestSettings AvoidScene(string name)
        {
            ScenesToAvoid ??= [];
            ScenesToAvoid.Add(name);
            return this;
        }
        public SceneRequestSettings AvoidScenes(IEnumerable<string> names)
        {
            ScenesToAvoid ??= [];
            ScenesToAvoid.AddRange(names);
            return this;
        }
        internal Dictionary<object, object>? Properties { get; set; }
        public SceneRequestSettings AddProperty<TKey, T>(TKey key, T value)
        {
            Properties ??= [];
            Properties.Add(key!, value!);
            return this;
        }
        internal SceneContext? Context { get; set; }
        public SceneRequestSettings InitizializeContext(out SceneContext context)
        {
            context = new SceneContext { InputMessage = null!, Properties = Properties ?? [] };
            return this;
        }
    }
}
