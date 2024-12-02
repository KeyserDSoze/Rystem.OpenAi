using System.IO;

namespace Rystem.OpenAi.Files
{
    public sealed class FileRequest : IOpenAiRequest
    {
        public Stream? File { get; set; }
        public string? Purpose { get; set; }
        public string? Model { get; set; }
    }
}
