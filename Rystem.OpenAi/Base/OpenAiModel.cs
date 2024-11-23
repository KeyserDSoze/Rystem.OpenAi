using System.Collections.Generic;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiModel
    {
        private readonly OpenAiModelName _model;
        public OpenAiModel(OpenAiModelName model)
            => _model = model;
        public string AsString()
            => _model;
        public List<OpenAiCost> Costs { get; } = [];
    }
}
