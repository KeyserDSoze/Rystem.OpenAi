using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageEditRequest : ImageVariationRequest
    {
        public MemoryStream? Mask { get; set; }
        public string? MaskName { get; set; }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Mask?.Close();
                Mask?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
