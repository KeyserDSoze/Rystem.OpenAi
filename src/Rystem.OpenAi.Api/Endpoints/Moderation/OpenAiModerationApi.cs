using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModerationApi : OpenAiBase, IOpenAiModerationApi
    {
        public OpenAiModerationApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations)
            : base(httpClientFactory, configurations)
        {
        }
        public ModerationRequestBuilder Create(string input)
            => new ModerationRequestBuilder(_client, _configuration, input);
    }
}
