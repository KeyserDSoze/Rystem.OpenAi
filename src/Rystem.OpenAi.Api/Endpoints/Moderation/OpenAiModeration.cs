using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModeration : OpenAiBase, IOpenAiModeration
    {
        public OpenAiModeration(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ModerationRequestBuilder Create(string input)
            => new ModerationRequestBuilder(Client, Configuration, input, Utility);
    }
}
