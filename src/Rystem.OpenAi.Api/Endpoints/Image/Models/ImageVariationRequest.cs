using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Image
{
    public sealed class ImageVariationRequest : IOpenAiRequest, IDisposable
    {
        public MemoryStream? Image { get; set; }
        public string? ImageName { get; set; }
        public int NumberOfResults { get; set; }
        public string? Size { get; set; }
        public string? User { get; set; }
        public string? ResponseFormat { get; set; }
        [JsonIgnore]
        public string? ModelId { get; set; }
        private void Dispose(bool disposing)
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
