using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public interface IOpenAiWithMetadata<T>
    {
        T AddMetadata(string key, string value);
        T AddMetadata(Dictionary<string, string> metadata);
        T ClearMetadata();
        T RemoveMetadata(string key);
    }
}
