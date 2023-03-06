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
    public sealed class ImageEditRequestBuilder : RequestBuilder<ImageEditRequest>
    {
        internal ImageEditRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string? prompt,
            Stream image, string imageName) :
            base(client, configuration, () =>
            {
                var request = new ImageEditRequest()
                {
                    Prompt = prompt,
                    NumberOfResults = 1,
                    Size = ImageSize.Large.AsString()
                };
                var memoryStream = new MemoryStream();
                image.CopyTo(memoryStream);
                request.Image = memoryStream;
                request.ImageName = imageName;
                return request;
            })
        {
        }
        /// <summary>
        /// Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async ValueTask<ImageResult> GetUrlAsync(CancellationToken cancellationToken = default)
        {
            _request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (_request.Image != null)
            {
                using var imageData = new MemoryStream();
                await _request.Image.CopyToAsync(imageData, cancellationToken);
                imageData.Position = 0;
                content.Add(new ByteArrayContent(imageData.ToArray()), "image", _request.ImageName);
            }
            if (_request.Mask != null)
            {
                using var maskData = new MemoryStream();
                await _request.Mask.CopyToAsync(maskData, cancellationToken);
                maskData.Position = 0;
                content.Add(new ByteArrayContent(maskData.ToArray()), "mask", _request.MaskName);
            }
            if (_request.Prompt != null)
                content.Add(new StringContent(_request.Prompt), "prompt");
            if (_request.NumberOfResults != null)
                content.Add(new StringContent(_request.NumberOfResults.ToString()), "n");
            if (_request.Size != null)
                content.Add(new StringContent(_request.Size), "size");

            if (!string.IsNullOrWhiteSpace(_request.User))
                content.Add(new StringContent(_request.User), "user");

            _request.Dispose();

            var response = await _client.PostAsync<ImageResult>($"{_configuration.GetUri(OpenAi.Image, _request.ModelId!)}/edits", content, cancellationToken);
            return response;
        }
        /// <summary>
        /// Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var uri = $"{_configuration.GetUri(OpenAi.Image, _request.ModelId!)}/generations";
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
        public ImageEditRequestBuilder WithNumberOfResults(int numberOfResults)
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
        public ImageEditRequestBuilder WithSize(ImageSize size)
        {
            _request.Size = size.AsString();
            return this;
        }
        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="maskName"></param>
        /// <returns></returns>
        public ImageEditRequestBuilder WithMask(Stream mask, string maskName = "mask.png")
        {
            var memoryStream = new MemoryStream();
            mask.CopyTo(memoryStream);
            _request.Mask = memoryStream;
            _request.MaskName = maskName;
            return this;
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public ImageEditRequestBuilder WithUser(string user)
        {
            _request.User = user;
            return this;
        }
    }
}
