using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImageApi : OpenAiBase, IOpenAiImageApi
    {
        public OpenAiImageApi(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
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
            return new ImageCreateRequestBuilder(_client, _configuration, prompt, _utility);
        }
        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        public ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(_client, _configuration, image, imageName, false, _utility);
        /// <summary>
        /// Creates a variation of a given image. Take the streamed image and transform it before sending in a correct png.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        public ImageVariationRequestBuilder VariateAndTransformInPng(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(_client, _configuration, image, imageName, true, _utility);
    }
}
