using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageEditRequest : IOpenAiRequest, IDisposable
    {
        public string? Prompt { get; set; }
        public MemoryStream? Image { get; set; }
        public string? ImageName { get; set; }
        public MemoryStream? Mask { get; set; }
        public string? MaskName { get; set; }
        public int NumberOfResults { get; set; }
        public string? Size { get; set; }
        public string? User { get; set; }
        public string? ResponseFormat { get; set; }
        public string? ModelId { get; set; }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Close();
                Image?.Dispose();
                Mask?.Dispose();
                Mask?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
