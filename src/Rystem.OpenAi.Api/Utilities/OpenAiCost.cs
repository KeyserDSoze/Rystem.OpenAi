using System.Collections.Generic;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiCost : IOpenAiCost
    {
        private readonly OpenAiCostSettings _price;
        public OpenAiCost(OpenAiSettings settings)
        {
            _price = settings.Price;
        }
        public decimal Get(List<int> tokens)
        {
            throw new System.NotImplementedException();
        }

        public decimal Get(int numberOfTokens)
        {
            throw new System.NotImplementedException();
        }
    }
}
