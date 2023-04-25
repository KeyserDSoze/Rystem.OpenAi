﻿using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Chat
{
    internal sealed class OpenAiChat : OpenAiBase, IOpenAiChat, IOpenAiChatApi
    {
        public OpenAiChat(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public ChatRequestBuilder Request(ChatMessage message)
            => new ChatRequestBuilder(Client, Configuration, message, Utility);
    }
}