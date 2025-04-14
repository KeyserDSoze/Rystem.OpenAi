using System.Linq.Expressions;

namespace Rystem.PlayFramework
{
    public interface ISceneServiceBuilder<T>
        where T : class
    {
        ISceneServiceBuilder<T> WithMethod(Expression<Func<T, Delegate>> method, string? name = null, string? description = null);
    }
}
