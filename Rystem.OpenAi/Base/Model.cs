using System.Collections.Generic;

namespace Rystem.OpenAi
{
    internal sealed class Model
    {
        private readonly ModelName _model;
        public Model(ModelName model)
            => _model = model;
        public string AsString()
            => _model;
        public List<OpenAiCost> Costs { get; } = [];
    }
}
