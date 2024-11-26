using System;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal sealed class OpenAiImage : OpenAiBuilder<IOpenAiImage, ImageRequest>, IOpenAiImage
    {
        public OpenAiImage(IFactory<DefaultServices> factory) : base(factory)
        {
        }
        private void CheckPromptLenght(string prompt)
        {
            if (prompt.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
        }
        public INewImageRequest Generate(string prompt)
        {
            CheckPromptLenght(prompt);
            return new ImageCreateRequestBuilder(_factory);
        }
        public ImageEditRequestBuilder Edit(string prompt)
        {
            CheckPromptLenght(prompt);
            return new ImageEditRequestBuilder(Client, _configuration, prompt, image, imageName, false, ImageSize.Large, Utility);
        }
        public ImageVariationRequestBuilder Variate()
            => new ImageVariationRequestBuilder(Client, _configuration, null, image, imageName, false, ImageSize.Large, Utility);

    }
}
