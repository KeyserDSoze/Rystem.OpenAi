using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public class ImageVariationRequest : ImageRequest, IDisposable
    {
        public MemoryStream? Image { get; set; }
        public string? ImageName { get; set; }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
