namespace Rystem.OpenAi
{
    public interface IOpenAiLoggerFactory
    {
        public IOpenAiLoggerFactory ConfigureTypes(params OpenAiType[] types);
        public IOpenAiLogger Create();
    }
}
