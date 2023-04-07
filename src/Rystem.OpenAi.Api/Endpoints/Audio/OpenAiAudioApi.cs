using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiAudioApi : OpenAiBase, IOpenAiAudioApi
    {
        public OpenAiAudioApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations)
            : base(httpClientFactory, configurations)
        {
        }
        public AudioRequestBuilder Request(Stream file, string fileName = "default")
            => new AudioRequestBuilder(_client, _configuration, file, fileName);
    }
}
