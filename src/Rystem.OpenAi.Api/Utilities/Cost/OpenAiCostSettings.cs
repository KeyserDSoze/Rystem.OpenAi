using System.Collections.Generic;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi
{
    public sealed class OpenAiPriceSettings
    {
        internal Dictionary<string, CostFormula> Settings { get; } = new Dictionary<string, CostFormula>();
        internal OpenAiPriceSettings()
        {
            SetGpt4With8KPrice();
            SetGpt4With32KPrice();
            SetGpt3_5();
            SetGpt3_5_16K();
            SetAdaEmbeddings();
            SetFineTuneForAda();
            SetFineTuneForBabbage();
            SetFineTuneForCurie();
            SetFineTuneForDavinci();
            SetImage(ImageSize.Large, 0.02M);
            SetImage(ImageSize.Medium, 0.018M);
            SetImage(ImageSize.Small, 0.016M);
            SetAudioForTranslation();
            SetAudioForTranscription();
            SetModeration();
        }
        private OpenAiPriceSettings Set(string key, CostFormula formula)
        {
            if (!Settings.ContainsKey(key))
                Settings.Add(key, formula);
            else
                Settings[key] = formula;
            return this;
        }
        internal OpenAiPriceSettings SetAzureDefault()
        {
            SetGpt4With8KPrice();
            SetGpt4With32KPrice();
            SetGpt3_5(0.002M, 0.002M);
            SetGpt3_5_16K();
            SetAdaEmbeddings(0.0004M);
            SetFineTuneForAda();
            SetFineTuneForBabbage();
            SetFineTuneForCurie();
            SetFineTuneForDavinci();
            SetImage(ImageSize.Large, 0.02M);
            SetImage(ImageSize.Medium, 0.02M);
            SetImage(ImageSize.Small, 0.02M);
            SetAudioForTranslation();
            SetAudioForTranscription();
            SetModeration();
            return this;
        }
        public OpenAiPriceSettings SetGpt4With8KPrice(decimal prompt = 0.03M, decimal completion = 0.06M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt4_8K}", new CostFormula
            {
                PromptUsage = prompt
            });
        }
        public OpenAiPriceSettings SetGpt4With32KPrice(decimal prompt = 0.06M, decimal completion = 0.12M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt4_32K}", new CostFormula
            {
                PromptUsage = prompt
            });
        }
        public OpenAiPriceSettings SetGpt3_5(decimal usage = 0.0015M, decimal completion = 0.002M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt3_5}", new CostFormula
            {
                Usage = usage,
                CompletionUsage = completion
            });
        }
        public OpenAiPriceSettings SetGpt3_5_16K(decimal usage = 0.003M, decimal completion = 0.004M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt3_5_16K}", new CostFormula
            {
                Usage = usage,
                CompletionUsage = completion
            });
        }
        public OpenAiPriceSettings SetAdaEmbeddings(decimal usage = 0.0001M)
        {
            var formula = new CostFormula
            {
                Usage = usage
            };
            return Set($"{OpenAiType.Embedding}_{ModelFamilyType.Ada}", formula);
        }
        public OpenAiPriceSettings SetFineTuneForAda(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTuning}_{ModelFamilyType.Ada}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForBabbage(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTuning}_{ModelFamilyType.Babbage}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForCurie(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTuning}_{ModelFamilyType.Curie}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForDavinci(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTuning}_{ModelFamilyType.Davinci}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetModeration(decimal usage = 0M)
        {
            return Set($"{OpenAiType.Moderation}_{ModelFamilyType.Moderation}", new CostFormula
            {
                Usage = usage
            });
        }

        public OpenAiPriceSettings SetImage(ImageSize size, decimal perUnit)
            => Set($"{OpenAiType.Image}_{size}", new CostFormula
            {
                PerUnit = perUnit,
            });
        public OpenAiPriceSettings SetAudioForTranslation(decimal perMinute = 0.006M)
        {
            return Set($"{OpenAiType.AudioTranslation}", new CostFormula
            {
                PerMinute = perMinute
            });
        }
        public OpenAiPriceSettings SetAudioForTranscription(decimal perMinute = 0.006M)
        {
            return Set($"{OpenAiType.AudioTranscription}", new CostFormula
            {
                PerMinute = perMinute
            });
        }
    }
}
