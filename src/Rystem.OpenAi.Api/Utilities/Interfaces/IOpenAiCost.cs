using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public interface IOpenAiCost
    {
        decimal Get(List<int> tokens);
        decimal Get(int numberOfTokens);
    }
}
