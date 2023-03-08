using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageVariationRequestBuilder : RequestBuilder<ImageVariationRequest>
    {
        internal ImageVariationRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            Stream image, string imageName, bool transform)
            : base(client, configuration, () =>
            {
                var request = new ImageVariationRequest
                {
                    NumberOfResults = 1,
                    Size = ImageSize.Large.AsString(),
                    ImageName = imageName
                };
                if (!transform)
                {
                    var memoryStream = new MemoryStream();
                    image.CopyTo(memoryStream);
                    request.Image = memoryStream;
                }
                else
                {
                    var original = (Bitmap)Bitmap.FromStream(image);
                    var clone = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);
                    using (var gr = Graphics.FromImage(clone))
                    {
                        gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
                    }
                    var memoryStream = new MemoryStream();
                    clone.Save(memoryStream, ImageFormat.Png);
                    request.Image = memoryStream;
                }
                request.Image.Position = 0;
                return request;
            })
        {
        }
        /// <summary>
        /// Variate an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (_request.Image != null)
                content.Add(new ByteArrayContent(_request.Image.ToArray()), "image", _request.ImageName);
            if (_request.NumberOfResults != null)
                content.Add(new StringContent(_request.NumberOfResults.ToString()), "n");
            if (_request.Size != null)
                content.Add(new StringContent(_request.Size), "size");

            if (!string.IsNullOrWhiteSpace(_request.User))
                content.Add(new StringContent(_request.User), "user");
            _request.Dispose();

            var response = await _client.PostAsync<ImageResult>($"{_configuration.GetUri(OpenAi.Image, _request.ModelId!, _forced)}/variations", content, cancellationToken);
            return response;
        }
        /// <summary>
        /// Variate an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var uri = $"{_configuration.GetUri(OpenAi.Image, _request.ModelId!, _forced)}/generations";
            var responses = await _client.PostAsync<ImageResult>(uri, _request, cancellationToken);
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
        public ImageVariationRequestBuilder WithNumberOfResults(int numberOfResults)
        {
            if (numberOfResults > 10 || numberOfResults < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
            _request.NumberOfResults = numberOfResults;
            return this;
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public ImageVariationRequestBuilder WithSize(ImageSize size)
        {
            _request.Size = size.AsString();
            return this;
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public ImageVariationRequestBuilder WithUser(string user)
        {
            _request.User = user;
            return this;
        }
    }
}
