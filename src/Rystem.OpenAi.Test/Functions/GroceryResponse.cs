using System.Collections.Generic;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class GroceryResponse
    {
        public List<GroceryInnerResponse> Food { get; set; }
    }
}
