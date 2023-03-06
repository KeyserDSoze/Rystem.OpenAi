using System;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImageApi : IOpenAiImageApi
    {
        private readonly HttpClient _client;
        private readonly OpenAiConfiguration _configuration;
        public OpenAiImageApi(IHttpClientFactory httpClientFactory, OpenAiConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient(OpenAiSettings.HttpClientName);
            _configuration = configuration;
        }
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>Generation Builder</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ImageCreateRequestBuilder Generate(string prompt)
        {
            if (prompt.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
            return new ImageCreateRequestBuilder(_client, _configuration, prompt);
        }
        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        public ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(_client, _configuration, image, imageName);
    }
}
