using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi
{
    public sealed class PriceBuilder
    {
        internal Dictionary<string, Model> Prices { get; } = [];
        internal PriceBuilder() { }
        public PriceBuilder AddModel(ModelName modelName, params OpenAiCost[] openAiCostModels)
            => UpdatePrice(modelName, openAiCostModels);
        public PriceBuilder UpdatePrice(ModelName modelName, params OpenAiCost[] openAiCostModels)
        {
            foreach (var openAiCostModel in openAiCostModels)
                if (Prices.ContainsKey(modelName))
                {
                    var model = Prices[modelName];
                    var costModel = model.Costs.FirstOrDefault(x => x.Kind == openAiCostModel.Kind);
                    if (costModel != null)
                        model.Costs.Remove(costModel);
                    model.Costs.Add(openAiCostModel);
                }
                else
                {
                    var model = new Model(modelName);
                    model.Costs.Add(openAiCostModel);
                    Prices.Add(modelName, model);
                }
            return this;
        }
        //todo: to complete the configuration from pricing and set up also the azure price
        public static PriceBuilder Default => new PriceBuilder()
            .AddModel(ChatModelName.Gpt4_o,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00000125m },
                new OpenAiCost { Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens, Units = 0.00001m })
            .AddModel(ChatModelName.Gpt_4o_2024_11_20,
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ChatModelName.Gpt_4o_2024_08_06,
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
    .AddModel(ChatModelName.Gpt_4o_audio_preview,
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.1m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.2m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ChatModelName.Gpt_4o_audio_preview_2024_10_01,
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.1m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.2m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
    .AddModel(ChatModelName.Gpt_4o_2024_05_13,
        new OpenAiCost { Units = 0.000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ChatModelName.Gpt_4o_mini,
        new OpenAiCost { Units = 0.00000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.000000075m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ChatModelName.O1_preview,
        new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.0000075m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ChatModelName.O1_mini,
        new OpenAiCost { Units = 0.000003m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.000012m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(EmbeddingModelName.Text_embedding_3_small,
        new OpenAiCost { Units = 0.00000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(EmbeddingModelName.Text_embedding_3_large,
        new OpenAiCost { Units = 0.00000013m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(RealTimeModelName.Gpt_4o_realtime_preview,
        new OpenAiCost { Units = 0.000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.00002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.1m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
        new OpenAiCost { Units = 0.2m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })

    .AddModel(ImageModelName.Dalle3,
        new OpenAiCost { Units = 0.04m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Images })

    .AddModel(AudioModelName.Whisper,
        new OpenAiCost { Units = 0.0001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Minutes })

    .AddModel(SpeechModelName.Tts,
        new OpenAiCost { Units = 0.015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Characters })

    .AddModel(SpeechModelName.TtsHd,
        new OpenAiCost { Units = 0.03m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Characters });
    }
}
