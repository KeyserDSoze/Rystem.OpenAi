using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Image
{
    public abstract class ImageRequestBuilder<TBuilder> : RequestBuilder<ImageRequest>, IImageRequestBuilder
        where TBuilder : IImageRequestBuilder
    {
        private protected ImageSize _size;
        private protected ImageRequestBuilder(HttpClient client,
            OpenAiConfiguration configuration,
            IOpenAiUtility utility,
            Func<ImageRequest> defaultRequestCreator)
            : base(client, configuration, () => defaultRequestCreator(), utility)
        {
            _familyType = ModelFamilyType.Image;
            _size = ImageSize.Large;
            Request.Size = _size.AsString();
        }
        private protected abstract object CreateRequest();
        private protected abstract string Endpoint { get; }
        /// <summary>
        /// Create, Variate or Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = FormatResultImage.Url.AsString();
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, Endpoint);
            return Client.PostAsync<ImageResult>(uri, CreateRequest(), Configuration, cancellationToken);
        }
        /// <summary>
        /// Create, Variate or Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of base64 images.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public ValueTask<ImageResultForBase64> ExecuteWithBase64Async(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = FormatResultImage.B64Json.AsString();
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, Endpoint);
            return Client.PostAsync<ImageResultForBase64>(uri, CreateRequest(), Configuration, cancellationToken);
        }
        /// <summary>
        /// Download N images as Stream after you create, variate or edit by a given prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var responses = await ExecuteWithBase64Async(cancellationToken);
            if (responses.Data != null)
            {
                foreach (var image in responses.Data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var stream = image.ConvertToStream();
                    if (stream != null)
                        yield return stream;
                }
            }
        }
        /// <summary>
        /// The number of images to generate. Must be between 1 and 10.
        /// </summary>
        /// <param name="numberOfResults"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TBuilder WithNumberOfResults(int numberOfResults)
        {
            if (numberOfResults > 10 || numberOfResults < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            Request.NumberOfResults = numberOfResults;
            return (TBuilder)(this as IImageRequestBuilder);
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public TBuilder WithSize(ImageSize size)
        {
            _size = size;
            Request.Size = size.AsString();
            return (TBuilder)(this as IImageRequestBuilder);
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public TBuilder WithUser(string user)
        {
            Request.User = user;
            return (TBuilder)(this as IImageRequestBuilder);
        }
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
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                ImageSize = _size,
                Units = Request.NumberOfResults
            });
        }
    }
}
