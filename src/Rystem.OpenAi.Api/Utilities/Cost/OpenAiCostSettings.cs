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
            SetAda();
            SetBabbage();
            SetCurie();
            SetDavinci();
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
        public OpenAiPriceSettings SetGpt4With8KPrice(decimal prompt = 0.03M, decimal completion = 0.06M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt4_32K}", new CostFormula
            {
                PromptUsage = prompt
            })
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Gpt4_32K}", new CostFormula
                {
                    CompletionUsage = completion,
                });
        }
        public OpenAiPriceSettings SetGpt4With32KPrice(decimal prompt = 0.06M, decimal completion = 0.12M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt4_32K}", new CostFormula
            {
                PromptUsage = prompt
            })
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Gpt4_32K}", new CostFormula
                {
                    CompletionUsage = completion,
                });
        }
        public OpenAiPriceSettings SetGpt3_5(decimal usage = 0.002M)
        {
            return Set($"{OpenAiType.Chat}_{ModelFamilyType.Gpt3_5}", new CostFormula
            {
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetAda(decimal usage = 0.0004M)
        {
            var formula = new CostFormula
            {
                Usage = usage
            };
            return
                Set($"{OpenAiType.Edit}_{ModelFamilyType.Ada}", formula)
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Ada}", formula);
        }
        public OpenAiPriceSettings SetBabbage(decimal usage = 0.0005M)
        {
            var formula = new CostFormula
            {
                Usage = usage
            };
            return
                Set($"{OpenAiType.Edit}_{ModelFamilyType.Babbage}", formula)
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Babbage}", formula);
        }
        public OpenAiPriceSettings SetCurie(decimal usage = 0.002M)
        {
            var formula = new CostFormula
            {
                Usage = usage
            };
            return
                Set($"{OpenAiType.Edit}_{ModelFamilyType.Curie}", formula)
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Curie}", formula);
        }
        public OpenAiPriceSettings SetDavinci(decimal usage = 0.02M)
        {
            var formula = new CostFormula
            {
                Usage = usage
            };
            return
                Set($"{OpenAiType.Edit}_{ModelFamilyType.Davinci}", formula)
                .Set($"{OpenAiType.Completion}_{ModelFamilyType.Davinci}", formula);
        }
        public OpenAiPriceSettings SetFineTuneForAda(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTune}_{ModelFamilyType.Ada}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForBabbage(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTune}_{ModelFamilyType.Babbage}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForCurie(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTune}_{ModelFamilyType.Curie}", new CostFormula
            {
                Training = training,
                Usage = usage
            });
        }
        public OpenAiPriceSettings SetFineTuneForDavinci(decimal training = 0.0004M, decimal usage = 0.0016M)
        {
            return Set($"{OpenAiType.FineTune}_{ModelFamilyType.Davinci}", new CostFormula
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
        //todo Add price from Azure https://prices.azure.com/api/retail/prices?api-version=2023-01-01-preview&meterRegion=%27primary%27
    }
}
