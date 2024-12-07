using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Rystem.PlayFramework
{
    internal sealed class SceneServiceBuilder<T> : ISceneServiceBuilder<T>
        where T : class
    {
        public List<MethodInfo> Methods { get; } = [];
        private static readonly Regex s_regex = new("(?<=Convert\\().*?(?=\\.CreateDelegate\\()");
        public ISceneServiceBuilder<T> WithMethod(Expression<Func<T, Delegate>> method)
        {
            var body = s_regex.Match(method.Body.ToString()).Value;
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var currentMethod = methods.FirstOrDefault(x => x.ToSignature() == body);
            if (currentMethod != null)
                Methods.Add(currentMethod);
            else
                throw new ArgumentNullException($"Method {body} not found in {typeof(T).Name}");
            return this;
        }
    }
}
