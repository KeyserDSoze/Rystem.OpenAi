namespace Rystem.OpenAi
{
    public interface IOpenAiFactoryNoDependencyInjection : IOpenAiFactory
    {
        IOpenAiUtility Utility();
    }
}
