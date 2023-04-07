using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiFactory : IOpenAiFactory
    {
        private readonly IOpenAiApi _openAiApi;

        public OpenAiFactory(IOpenAiApi openAiApi)
        {
            _openAiApi = openAiApi;
        }
        public IOpenAiApi Create(string? name = default)
        {
            if (name == null)
                name = string.Empty;
            if (_openAiApi is OpenAiApi internalImplementation)
                internalImplementation.SetName(name);
            return _openAiApi;
        }
    }
}
