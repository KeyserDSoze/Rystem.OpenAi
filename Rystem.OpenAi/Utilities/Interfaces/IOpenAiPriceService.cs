namespace Rystem.OpenAi
{
    public interface IOpenAiPriceService
    {
        decimal CalculatePrice(OpenAiModelName modelName, params OpenAiCost[] spentRequests);
    }
}
