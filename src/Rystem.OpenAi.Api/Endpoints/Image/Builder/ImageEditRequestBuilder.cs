using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageEditRequestBuilder : ImageRequestBuilder<ImageEditRequestBuilder, ImageEditRequest>
    {
        internal ImageEditRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string? prompt,
            Stream image, string imageName, bool transform, ImageSize size, IOpenAiUtility utility) :
            base(client, configuration, utility, () =>
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
            })
        {
            _size = size;
        }
        private protected override object CreateRequest()
        {
            var content = new MultipartFormDataContent();
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
            return content;
        }
        private const string Edits = "/edits";
        private protected override string Endpoint => Edits;
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
    }
}
