using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;

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
        private protected override object CreateRequest()
        {
            var content = new MultipartFormDataContent();
            if (Request.Image != null)
                content.Add(new ByteArrayContent(Request.Image.ToArray()), "image", Request.ImageName);
            content.Add(new StringContent(Request.NumberOfResults.ToString()), "n");
            if (Request.Size != null)
                content.Add(new StringContent(Request.Size), "size");

            if (!string.IsNullOrWhiteSpace(Request.User))
                content.Add(new StringContent(Request.User), "user");
            Request.Dispose();
            return content;
        }
        private const string Variations = "/variations";
        private protected override string Endpoint => Variations;
    }
}
