using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModerationApi : OpenAiBase, IOpenAiModerationApi
    {
        public OpenAiModerationApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ModerationRequestBuilder Create(string input)
            => new ModerationRequestBuilder(_client, _configuration, input, _utility);
    }
}
