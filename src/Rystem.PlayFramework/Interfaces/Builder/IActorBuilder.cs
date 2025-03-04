namespace Rystem.PlayFramework
{
    public interface IActorBuilder
    {
        IActorBuilder AddActor<T>() where T : class, IActor;
        IActorBuilder AddActor<T>(string name, string role, Action<Dictionary<string, string>> parameters) where T : class, IActor;
        IActorBuilder AddActor(string role);
        IActorBuilder AddActor(Func<SceneContext, string> action);
        IActorBuilder AddActor(Func<SceneContext, CancellationToken, Task<string>> action);
    }
}
