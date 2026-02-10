namespace Rystem.PlayFramework
{
    /// <summary>
    /// Handler to track actor information for planning purposes
    /// </summary>
    internal sealed class ActorsHandler
    {
        private Dictionary<string, List<ActorInfo>> ActorsByScene { get; } = [];

        /// <summary>
        /// Add actor info for a scene
        /// </summary>
        public void AddActorInfo(string sceneName, string? role, string? typeName = null)
        {
            if (!ActorsByScene.ContainsKey(sceneName))
            {
                ActorsByScene[sceneName] = [];
            }
            
            ActorsByScene[sceneName].Add(new ActorInfo
            {
                Role = role,
                TypeName = typeName
            });
        }

        /// <summary>
        /// Get actor infos for a specific scene
        /// </summary>
        public IEnumerable<ActorInfo> GetActorInfos(string sceneName)
        {
            if (ActorsByScene.TryGetValue(sceneName, out var actors))
            {
                return actors;
            }
            return [];
        }

        /// <summary>
        /// Get actor infos for main actors (play every scene)
        /// </summary>
        public IEnumerable<ActorInfo> GetMainActorInfos()
        {
            if (ActorsByScene.TryGetValue(ScenesBuilder.MainActor, out var actors))
            {
                return actors;
            }
            return [];
        }
    }

    /// <summary>
    /// Information about an actor for planning
    /// </summary>
    internal sealed class ActorInfo
    {
        /// <summary>
        /// The role/description of the actor (what context it provides)
        /// </summary>
        public string? Role { get; init; }

        /// <summary>
        /// The type name of the actor (for custom actors)
        /// </summary>
        public string? TypeName { get; init; }
    }
}
