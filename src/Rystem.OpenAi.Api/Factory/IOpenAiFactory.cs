namespace Rystem.OpenAi
{
    public interface IOpenAiFactory
    {
        IOpenAiApi Create(string? name = default);
    }
}
