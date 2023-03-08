using System;
using System.IO;

namespace Rystem.OpenAi.Image
{
    public sealed class AudioRequest : IOpenAiRequest, IDisposable
    {
        public MemoryStream? Audio { get; set; }
        public string? AudioName { get; set; }
        public string? ModelId { get; set; }
        public string? Prompt { get; set; }
        public string? ResponseFormat { get; set; }
        public double? Temperature { get; set; }
        public string? Language { get; set; }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Audio?.Close();
                Audio?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
