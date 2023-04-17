using System;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiCost : IOpenAiCost
    {
        private readonly OpenAiPriceSettings _price;
        public OpenAiCost(OpenAiPriceSettings price)
        {
            _price = price;
        }
        public CostCalculation Configure(Action<OpenAiCostBuilder> action)
        {
            var costBuilder = new OpenAiCostBuilder(_price);
            action.Invoke(costBuilder);
            return costBuilder.Calculate();
        }
    }
}
