﻿using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Rystem.OpenAi.Moderation
{
    internal sealed class OpenAiModeration : OpenAiBase, IOpenAiModeration
    {
        public OpenAiModeration(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ModerationRequestBuilder Create(string input)
            => new ModerationRequestBuilder(Client, _configuration, input, Utility);
    }
}
