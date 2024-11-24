using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageCreateRequestBuilder : ImageRequestBuilder<ImageCreateRequestBuilder>
    {
        internal ImageCreateRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            string prompt, IOpenAiUtility utility)
            : base(client, configuration, utility, () =>
            {
                return new ImageRequest()
                {
                    Prompt = prompt,
                    NumberOfResults = 1,
                    Size = ImageSize.Large.AsString(),
                    ResponseFormat = FormatResultImage.Url.AsString(),
                };
            })
        {
        }
        private protected override object CreateRequest() => Request;
        private const string Generations = "/generations";
        private protected override string Endpoint => Generations;
        /// <summary>
        /// The image to use as the basis for the edit(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns>Edit Builder</returns>
        public ImageEditRequestBuilder Edit(Stream image, string imageName = "image.png")
            => new ImageEditRequestBuilder(Client, Configuration, Request.Prompt, image, imageName, false, _size, Utility);
        /// <summary>
        /// The image to use as the basis for the edit(s). Must be a valid PNG file, less than 4MB, and square.
        /// Take the streamed image and transform it before sending in a correct png.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageName"></param>
        /// <returns>Edit Builder</returns>
        public ImageEditRequestBuilder EditAndTrasformInPng(Stream image, string imageName = "image.png")
            => new ImageEditRequestBuilder(Client, Configuration, Request.Prompt, image, imageName, true, _size, Utility);
    }
}
