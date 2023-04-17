using System;

namespace Rystem.OpenAi
{
    public interface IOpenAiCost
    {
        CostCalculation Configure(Action<OpenAiCostBuilder> action);
    }
}
