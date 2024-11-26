using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal abstract class ImageRequestBuilder<TBuilder> : OpenAiBuilder<IOpenAiImage, ImageRequest>, INewImageRequest
        where TBuilder : INewImageRequest
    {
        private protected ImageSize _size;
        private protected ImageQuality _quality;
        private protected ImageRequestBuilder(IFactory<DefaultServices> factory) : base(factory)
        {
        }
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
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.Image, Request.Model!, Forced, Endpoint);
            return DefaultServices.HttpClient.PostAsync<ImageResult>(uri, Request, DefaultServices.Configuration, cancellationToken);
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
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.Image, Request.Model!, Forced, Endpoint);
            return DefaultServices.HttpClient.PostAsync<ImageResultForBase64>(uri, Request, DefaultServices.Configuration, cancellationToken);
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
            ChangeUsage();
            return (TBuilder)(this as INewImageRequest);
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public TBuilder WithSize(ImageSize size, ImageQuality quality)
        {
            _size = size;
            _quality = quality;
            ChangeUsage();
            Request.Size = size.AsString();
            Request.Quality = quality.AsString();
            return (TBuilder)(this as INewImageRequest);
        }
        private void ChangeUsage()
        {
            Usages.Clear();
            Usages.Add(new OpenAiCost { Units = Request.NumberOfResults, UnitOfMeasure = UnitOfMeasure.Images, Kind = _size.AsCost(_quality) });
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
            return (TBuilder)(this as INewImageRequest);
        }
    }
}
