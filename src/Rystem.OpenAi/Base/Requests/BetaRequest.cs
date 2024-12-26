using System.Collections.Generic;

namespace Rystem.OpenAi
{
    internal static class BetaRequest
    {
        public static readonly Dictionary<string, string> OpenAiBetaHeaders = new()
        {
            { "OpenAI-Beta", "assistants=v2" }
        };
    }
}
