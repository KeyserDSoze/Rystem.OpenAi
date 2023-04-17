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
    public sealed class ImageCreateRequestBuilder : RequestBuilder<ImageCreateRequest>
    {
        private ImageSize _size;
        internal ImageCreateRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string prompt, IOpenAiUtility utility)
            : base(client, configuration, () =>
            {
                return new ImageCreateRequest()
                {
                    Prompt = prompt,
                    NumberOfResults = 1,
                    Size = ImageSize.Large.AsString(),
                    ResponseFormat = ResponseFormatUrl,
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Image;
            _size = ImageSize.Large;
        }
        internal const string ResponseFormatUrl = "url";
        internal const string ResponseFormatB64Json = "b64_json";
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatUrl;
            var uri = $"{Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced)}/generations";
            return Client.PostAsync<ImageResult>(uri, Request, Configuration, cancellationToken);
        }
        /// <summary>
        /// Creates an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
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
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        /// <param name="numberOfResults"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ImageCreateRequestBuilder WithNumberOfResults(int numberOfResults)
        {
            if (numberOfResults > 10 || numberOfResults < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            Request.NumberOfResults = numberOfResults;
            return this;
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public ImageCreateRequestBuilder WithSize(ImageSize size)
        {
            _size = size;
            Request.Size = size.AsString();
            return this;
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public ImageCreateRequestBuilder WithUser(string user)
        {
            Request.User = user;
            return this;
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
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var cost = Utility.Cost;
            return cost.Configure(settings =>
            {
                settings
                    .WithType(OpenAiType.Image)
                    .WithImageSize(_size);
            }).Invoke(new OpenAiUsage
            {
                ImageSize = _size,
                Units = Request.NumberOfResults
            });
        }
    }
}
