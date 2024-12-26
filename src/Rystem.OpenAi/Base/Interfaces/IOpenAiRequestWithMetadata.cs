using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public interface IOpenAiRequestWithMetadata : IOpenAiRequest
    {
        Dictionary<string, string>? Metadata { get; set; }
    }
}
