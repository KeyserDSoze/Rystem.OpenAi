namespace Rystem.OpenAi
{
    public interface IOpenAiPriceService
    {
        decimal CalculatePrice(ModelName modelName, params OpenAiCost[] spentRequests);
    }
}
