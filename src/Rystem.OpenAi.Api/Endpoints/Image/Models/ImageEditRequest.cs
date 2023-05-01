using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageEditRequest : ImageVariationRequest
    {
        public MemoryStream? Mask { get; set; }
        public string? MaskName { get; set; }
        private protected override void InternalDispose(bool disposing)
        {
            if (disposing)
            {
                Mask?.Close();
                Mask?.Dispose();
            }
            base.InternalDispose(disposing);
        }
    }
}
