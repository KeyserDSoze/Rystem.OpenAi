using System;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiCost : IOpenAiCost
    {
        private readonly OpenAiPriceList _priceList;
        public OpenAiCost(OpenAiPriceList priceList)
        {
            _priceList = priceList;
        }
        public CostCalculation Configure(Action<OpenAiCostBuilder> action, string integrationName)
        {
            var costBuilder = new OpenAiCostBuilder(_priceList.Prices[integrationName]);
            action.Invoke(costBuilder);
            return costBuilder.Calculate();
        }
    }
}
