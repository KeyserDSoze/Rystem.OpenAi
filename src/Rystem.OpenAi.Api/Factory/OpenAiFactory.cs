using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiFactory : IOpenAiFactory
    {
        private readonly IEnumerable<IOpenAiApi> _openAis;
        private static readonly Dictionary<string, int> s_setupCounter = new Dictionary<string, int>();
        private static int _counter;
        internal static void AddCounter(string? name)
        {
            if (name == default)
                name = string.Empty;
            s_setupCounter.Add(name, _counter);
            _counter++;
        }
        public OpenAiFactory(IEnumerable<IOpenAiApi> openAis)
        {
            _openAis = openAis;
        }
        public IOpenAiApi Create(string? name = default)
        {
            if (name == null)
                name = string.Empty;
            return _openAis.Skip(s_setupCounter[name]).Take(1).First();
        }
    }
}
