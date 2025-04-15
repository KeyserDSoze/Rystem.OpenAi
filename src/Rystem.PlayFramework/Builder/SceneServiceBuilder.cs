using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    internal sealed class SceneServiceBuilder<T> : ISceneServiceBuilder<T>
        where T : class
    {
        public List<MethodBringer> Methods { get; } = [];
        private static readonly Regex s_regex = new("(?<=Convert\\().*?(?=\\.CreateDelegate\\()");
        public ISceneServiceBuilder<T> WithMethod(Expression<Func<T, Delegate>> method, string? name = null, string? description = null)
        {
            var body = s_regex.Match(method.Body.ToString()).Value;
            var currentMethod = FindMethodInInterfaces(typeof(T), body);
            if (currentMethod != null)
                Methods.Add(new MethodBringer(currentMethod, name, description));
            else
                throw new ArgumentNullException($"Method {body} not found in {typeof(T).Name}");
            return this;
        }
        public static string ToSignature(MethodInfo methodInfo)
            => $"{methodInfo.ReturnParameter?.ToString().Trim() ?? "void"} {methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(x => x.ParameterType.FullName))})";
        //search inside the type if exists further interfaces where to find the method in recursive
        private static MethodInfo? FindMethodInInterfaces(Type type, string body)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var method = methods.FirstOrDefault(x => ToSignature(x) == body);
            if (method != null)
                return method;
            foreach (var @interface in type.GetInterfaces())
            {
                method = FindMethodInInterfaces(@interface, body);
                if (method != null)
                    return method;
            }
            return null;
        }
    }
}
