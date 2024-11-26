using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Image
{
    internal sealed class ImageEditRequestBuilder : ImageModificationRequestBuilder<ImageEditRequestBuilder>
    {
        internal ImageEditRequestBuilder(IFactory<DefaultServices> factory) :
            base(factory)
        { }
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
            _mask = CalculateBytesFromImage(false, mask);
            _maskName = maskName;
            return this;
        }
        /// <summary>
        /// An additional image whose fully transparent areas (e.g. where alpha is zero) indicate where image should be edited. Must be a valid PNG file, less than 4MB, and have the same dimensions as image.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="maskName"></param>
        /// <returns></returns>
        public ImageEditRequestBuilder WithMask(params ImageRange[] masks)
        {
            var imageSize = _size switch
            {
                ImageSize.Small => 256,
                ImageSize.Medium => 512,
                _ => 1024,
            };
            using var memoryStream = new MemoryStream();
            using var clone = new Bitmap(imageSize, imageSize, PixelFormat.Format32bppPArgb);
            for (var i = 0; i < imageSize; i++)
            {
                for (var j = 0; j < imageSize; j++)
                {
                    if (masks.Any(x => i >= x.Horizontal.Start.Value && i <= x.Horizontal.End.Value
                    && j >= x.Vertical.Start.Value && j <= x.Vertical.End.Value))
                        clone.SetPixel(i, j, Color.Transparent);
                    else
                        clone.SetPixel(i, j, Color.Black);
                }
            }
            clone.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            _mask = new ByteArrayContent(memoryStream.ToArray());
            _maskName = "mask.png";
            return this;
        }
    }
}
