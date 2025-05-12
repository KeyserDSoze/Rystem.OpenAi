namespace Rystem.PlayFramework
{
    public sealed class SceneRequestSettings
    {
        internal string? Key { get; set; }
        internal List<string>? ScenesToAvoid { get; set; }
        internal bool CacheIsAvoidable { get; set; }
        public SceneRequestSettings WithKey(string key)
        {
            Key = key;
            return this;
        }
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
        public SceneRequestSettings AvoidCache()
        {
            CacheIsAvoidable = true;
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
    }
}
