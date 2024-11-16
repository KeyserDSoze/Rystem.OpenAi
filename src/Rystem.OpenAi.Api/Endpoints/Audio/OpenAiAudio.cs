﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiAudio : OpenAiBase, IOpenAiAudio
    {
        public OpenAiAudio(IHttpClientFactory httpClientFactory,
            IEnumerable<OpenAiConfiguration> configurations,
            IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        public AudioRequestBuilder Request(Stream file, string fileName = "default")
            => new AudioRequestBuilder(Client, _configuration, file, fileName, Utility);
        public AudioSpeechRequestBuilder Speech(string input)
            => new AudioSpeechRequestBuilder(Client, _configuration, input, Utility);
    }
}
