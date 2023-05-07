using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImage : OpenAiBase, IOpenAiImage
    {
        public OpenAiImage(IHttpClientFactory httpClientFactory, IEnumerable<OpenAiConfiguration> configurations, IOpenAiUtility utility)
            : base(httpClientFactory, configurations, utility)
        {
        }
        private void CheckPromptLenght(string prompt)
        {
            if (prompt.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
        }
        public ImageCreateRequestBuilder Generate(string prompt)
        {
            CheckPromptLenght(prompt);
            return new ImageCreateRequestBuilder(Client, _configuration, prompt, Utility);
        }
        public ImageEditRequestBuilder Edit(string prompt, Stream image, string imageName = "image.png")
        {
            CheckPromptLenght(prompt);
            return new ImageEditRequestBuilder(Client, _configuration, prompt, image, imageName, false, ImageSize.Large, Utility);
        }
        public ImageEditRequestBuilder EditAndTrasformInPng(string prompt, Stream image, string imageName = "image.png")
        {
            CheckPromptLenght(prompt);
            return new ImageEditRequestBuilder(Client, _configuration, prompt, image, imageName, true, ImageSize.Large, Utility);
        }
        public ImageVariationRequestBuilder Variate(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(Client, _configuration, null, image, imageName, false, ImageSize.Large, Utility);
        public ImageVariationRequestBuilder VariateAndTransformInPng(Stream image, string imageName = "image.png")
            => new ImageVariationRequestBuilder(Client, _configuration, null, image, imageName, true, ImageSize.Large, Utility);
    }
}
