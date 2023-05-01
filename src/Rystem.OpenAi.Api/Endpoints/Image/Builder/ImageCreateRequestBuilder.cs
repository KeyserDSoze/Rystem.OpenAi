using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageCreateRequestBuilder : ImageRequestBuilder<ImageCreateRequestBuilder, ImageRequest>
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
                    ResponseFormat = ResponseFormatUrl,
                };
            })
        {
        }
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public override ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatUrl;
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, "/generations");
            return Client.PostAsync<ImageResult>(uri, Request, Configuration, cancellationToken);
        }
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public override async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var responses = await ExecuteAsync(cancellationToken);
            if (responses.Data != null)
            {
                using var client = new HttpClient();
                foreach (var image in responses.Data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var response = await client.GetAsync(image.Url);
                    response.EnsureSuccessStatusCode();
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        using var stream = await response.Content.ReadAsStreamAsync();
                        var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        yield return memoryStream;
                    }
                }
            }
        }
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
