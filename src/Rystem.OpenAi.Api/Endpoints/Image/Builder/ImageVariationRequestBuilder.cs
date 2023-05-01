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
    public class ImageVariationRequestBuilder : ImageRequestBuilder<ImageVariationRequestBuilder, ImageVariationRequest>
    {
        internal ImageVariationRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            Stream image, string imageName, bool transform, IOpenAiUtility utility)
            : base(client, configuration, utility, () =>
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
        public override async ValueTask<ImageResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (Request.Image != null)
                content.Add(new ByteArrayContent(Request.Image.ToArray()), "image", Request.ImageName);
            content.Add(new StringContent(Request.NumberOfResults.ToString()), "n");
            if (Request.Size != null)
                content.Add(new StringContent(Request.Size), "size");

            if (!string.IsNullOrWhiteSpace(Request.User))
                content.Add(new StringContent(Request.User), "user");
            Request.Dispose();
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, "/variations");
            var response = await Client.PostAsync<ImageResult>(uri, content, Configuration, cancellationToken);
            return response;
        }
        /// <summary>
        /// Variate an image given a prompt.
        /// </summary>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of generated texture urls to download.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public override async IAsyncEnumerable<Stream> DownloadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var uri = Configuration.GetUri(OpenAiType.Image, Request.ModelId!, _forced, "/variations");
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
    }
}
