﻿using System.Collections.Generic;
using System.Net.Http;

namespace Rystem.OpenAi.Edit
{
    internal sealed class OpenAiEdit : OpenAiBase, IOpenAiEdit, IOpenAiEditApi
    {
        public OpenAiEdit(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public EditRequestBuilder Request(string instruction)
            => new EditRequestBuilder(Client, Configuration, instruction, Utility);
    }
}