using System.Drawing.Printing;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi
{
    public sealed class OpenAiUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int Minutes { get; set; }
        public int Units { get; set; }
        public ImageSize ImageSize { get; set; }
    }
}
