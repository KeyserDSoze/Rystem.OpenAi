using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    internal sealed class ScenePathBuilder : IScenePathBuilder
    {
        public List<Regex> RegexForApiMapping { get; set; } = new();
        public IScenePathBuilder Map(Regex regex)
        {
            RegexForApiMapping.Add(regex);
            return this;
        }
        public IScenePathBuilder Map(string startsWith)
        {
            RegexForApiMapping.Add(new Regex($"{startsWith}*"));
            return this;
        }
    }
}
