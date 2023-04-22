using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private ImageSize _size;
        internal ImageEditRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string? prompt,
            Stream image, string imageName, bool transform, ImageSize size, IOpenAiUtility utility) :
            base(client, configuration, () =>
            {
                var request = new ImageEditRequest
                {
                    Prompt = prompt,
                    NumberOfResults = 1,
                    Size = size.AsString(),
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
            }, utility)
        {
            _size = size;
        }
        /// <summary>
        /// Edit an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (Request.Image != null)
                content.Add(new ByteArrayContent(Request.Image.ToArray()), "image", Request.ImageName);
            if (Request.Mask != null)
                content.Add(new ByteArrayContent(Request.Mask.ToArray()), "mask", Request.MaskName);
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), "prompt");
            content.Add(new StringContent(Request.NumberOfResults.ToString()), "n");
            if (Request.Size != null)
                content.Add(new StringContent(Request.Size), "size");

            if (!string.IsNullOrWhiteSpace(Request.User))
                content.Add(new StringContent(Request.User), "user");

            Request.Dispose();

            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, "/edits");
            var response = await Client.PostAsync<ImageResult>(uri, content, Configuration, cancellationToken);
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
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, "/edits");
            var responses = await Client.PostAsync<ImageResult>(uri, Request, Configuration, cancellationToken);
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
            Request.NumberOfResults = numberOfResults;
            return this;
        }
        /// <summary>
        /// The size of the generated images. Must be one of 256x256, 512x512, or 1024x1024.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public ImageEditRequestBuilder WithSize(ImageSize size)
        {
            _size = size;
            Request.Size = size.AsString();
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
            Request.Mask = memoryStream;
            Request.MaskName = maskName;
            Request.Mask.Position = 0;
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
            Request.User = user;
            return this;
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
                    .WithFamily(_familyType)
                    .WithType(OpenAiType.Image);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                ImageSize = _size,
                Units = Request.NumberOfResults
            });
        }
    }
}
