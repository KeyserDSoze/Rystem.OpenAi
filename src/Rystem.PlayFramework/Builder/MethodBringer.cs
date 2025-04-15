using System.Reflection;
using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    internal sealed class MethodBringer
    {
        private static readonly Regex s_checkName = new("[^a-zA-Z0-9_\\-\\.]{1,64}");
        public MethodBringer(MethodInfo info, string? name = null, string? description = null)
        {
            Info = info;
            Name = name != null ? s_checkName.Replace(name.Replace(" ", "-"), string.Empty) : s_checkName.Replace(info.Name, string.Empty);
            Description = description;
        }
        public MethodInfo Info { get; }
        public string? Name { get; }
        public string? Description { get; }
    }
}
