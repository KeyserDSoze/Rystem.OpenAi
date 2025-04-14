using System.Reflection;

namespace Rystem.PlayFramework
{
    internal sealed class MethodBringer
    {
        public MethodBringer(MethodInfo info, string? name = null, string? description = null)
        {
            Info = info;
            Name = name;
            Description = description;
        }
        public MethodInfo Info { get; }
        public string? Name { get; }
        public string? Description { get; }
    }
}
