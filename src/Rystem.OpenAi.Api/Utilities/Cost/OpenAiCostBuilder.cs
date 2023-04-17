using System.Collections.Generic;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi
{
    public delegate decimal CostCalculation(OpenAiUsage usage);
    public sealed class OpenAiCostBuilder
    {
        private OpenAiType? _openAiType;
        private ModelFamilyType? _modelFamilyType;
        private ImageSize? _imageSize;
        private bool _isTraining;
        private readonly OpenAiPriceSettings _price;
        internal OpenAiCostBuilder(OpenAiPriceSettings price)
        {
            _price = price;
        }
        public OpenAiCostBuilder WithType(OpenAiType type)
        {
            _openAiType = type;
            return this;
        }
        public OpenAiCostBuilder WithFamily(ModelFamilyType familyType)
        {
            _modelFamilyType = familyType;
            return this;
        }
        public OpenAiCostBuilder WithImageSize(ImageSize size)
        {
            _imageSize = size;
            return this;
        }
        public OpenAiCostBuilder ForTraining()
        {
            _isTraining = true;
            return this;
        }
        internal CostCalculation Calculate()
        {
            var forKeys = new List<string>();
            if (_openAiType != null)
                forKeys.Add(_openAiType.Value.ToString());
            if (_modelFamilyType != null)
                forKeys.Add(_modelFamilyType.Value.ToString());
            if (_imageSize != null)
                forKeys.Add(_imageSize.Value.ToString());
            var forCalculation = _price.Settings[string.Join("_", forKeys)];
            return new CostCalculation((usage)
                => forCalculation.Calculate(_isTraining ? usage.PromptTokens : 0, usage.PromptTokens + usage.CompletionTokens, usage.Minutes, usage.Units, usage.PromptTokens, usage.CompletionTokens));
        }
    }
}
