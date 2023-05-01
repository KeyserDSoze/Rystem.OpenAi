using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public class ImageVariationRequest : ImageRequest, IDisposable
    {
        public MemoryStream? Image { get; set; }
        public string? ImageName { get; set; }
        private protected virtual void InternalDispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
            }
        }
        public void Dispose()
        {
            InternalDispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
