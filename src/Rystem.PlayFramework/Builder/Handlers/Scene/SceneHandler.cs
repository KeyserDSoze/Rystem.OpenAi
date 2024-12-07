using System.Text.RegularExpressions;
using Rystem.OpenAi.Chat;

namespace Rystem.PlayFramework
{
    internal sealed class SceneHandler
    {
        public Action<IOpenAiChat>? Chooser { get; set; }
        public List<Regex> AvailableApiPath { get; } = [];
        public List<string> Functions { get; } = [];
    }
}
