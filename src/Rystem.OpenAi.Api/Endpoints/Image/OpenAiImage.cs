using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImage : OpenAiBase, IOpenAiImage, IOpenAiImageApi
    {
        public OpenAiImage(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
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
            return new ImageCreateRequestBuilder(Client, Configuration, prompt, Utility);
        }
        /// <summary>
        /// Creates a variation of a given image.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        public ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(Client, Configuration, image, imageName, false, Utility);
        /// <summary>
        /// Creates a variation of a given image. Take the streamed image and transform it before sending in a correct png.
        /// </summary>
        /// <param name="image">The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.</param>
        /// <param name="imageName"></param>
        /// <returns>Variation Builder</returns>
        public ImageVariationRequestBuilder VariateAndTransformInPng(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(Client, Configuration, image, imageName, true, Utility);
    }
}
