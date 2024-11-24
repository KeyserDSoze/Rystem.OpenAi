using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;

namespace Rystem.OpenAi.Image
{
    public abstract class ImageModificationRequestBuilder<TBuilder> : ImageRequestBuilder<TBuilder>
        where TBuilder : IImageRequestBuilder
    {
        private readonly ByteArrayContent? _image;
        private readonly string? _imageName;
        private protected ByteArrayContent? _mask;
        private protected string? _maskName;
        private protected ImageModificationRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string? prompt,
            Stream image, string imageName, bool transform, ImageSize size, IOpenAiUtility utility) :
            base(client, configuration, utility, () =>
            {
                var request = new ImageRequest
                {
                    Prompt = prompt,
                    NumberOfResults = 1,
                    Size = size.AsString(),
                };
                return request;
            })
        {
            _size = size;
            _image = CalculateBytesFromImage(transform, image);
            _imageName = imageName;
        }
        private protected ByteArrayContent CalculateBytesFromImage(bool transform, Stream image)
        {
            using var memoryStream = new MemoryStream();
            if (!transform)
            {
                image.CopyTo(memoryStream);
            }
            else
            {
                using var original = (Bitmap)Bitmap.FromStream(image);
                using var clone = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppPArgb);
                _ = WithSize(original.Width switch
                {
                    512 => ImageSize.Medium,
                    256 => ImageSize.Small,
                    _ => ImageSize.Large,
                });
                using (var gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(original, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                clone.Save(memoryStream, ImageFormat.Png);
            }
            memoryStream.Position = 0;
            return new ByteArrayContent(memoryStream.ToArray());
        }
        private const string ImageLabel = "image";
        private const string MaskLabel = "mask";
        private const string PromptLabel = "prompt";
        private const string NumberOfImagesLabel = "n";
        private const string SizeLabel = "size";
        private const string ResponseFormatLabel = "response_format";
        private const string UserLabel = "user";
        private protected override object CreateRequest()
        {
            var content = new MultipartFormDataContent();
            if (_image != null)
                content.Add(_image, ImageLabel, _imageName);
            if (_mask != null)
                content.Add(_mask, MaskLabel, _maskName);
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), PromptLabel);
            content.Add(new StringContent(Request.NumberOfResults.ToString()), NumberOfImagesLabel);
            if (Request.Size != null)
                content.Add(new StringContent(Request.Size), SizeLabel);
            content.Add(new StringContent(Request.ResponseFormat), ResponseFormatLabel);
            if (!string.IsNullOrWhiteSpace(Request.User))
                content.Add(new StringContent(Request.User), UserLabel);
            return content;
        }
    }
}
