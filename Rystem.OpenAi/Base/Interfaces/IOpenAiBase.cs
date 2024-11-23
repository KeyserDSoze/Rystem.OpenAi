namespace Rystem.OpenAi
{
    public interface IOpenAiBase<T>
    {
        T WithModel(OpenAiModelName model);
        T WithModel(string model);
    }
}
