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

        // Default pricing based on OpenAI Standard tier (per 1M tokens)
        public static PriceBuilder Default => new PriceBuilder()
            // GPT-5 Series
            .AddModel(ChatModelName.Gpt_5_1,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_mini,
                new OpenAiCost { Units = 0.00000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_nano,
                new OpenAiCost { Units = 0.00000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000005m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_pro,
                new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00012m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_1_chat_latest,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_chat_latest,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_1_codex,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_codex,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_1_codex_mini,
                new OpenAiCost { Units = 0.00000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_5_search_api,
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // GPT-4.1 Series
            .AddModel(ChatModelName.Gpt_4_1,
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000005m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000008m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_1_mini,
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000001m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000016m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_1_nano,
                new OpenAiCost { Units = 0.0000001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // GPT-4o Series
            .AddModel(ChatModelName.Gpt_4o,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_2024_11_20,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_2024_08_06,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000125m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_2024_05_13,
                new OpenAiCost { Units = 0.000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_mini,
                new OpenAiCost { Units = 0.00000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000075m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // GPT Realtime/Audio (text tokens)
            .AddModel(ChatModelName.Gpt_realtime,
                new OpenAiCost { Units = 0.000004m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000016m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.032m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.064m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_realtime_mini,
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000006m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000024m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.01m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.02m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_audio,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.032m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.064m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_audio_mini,
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000024m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.01m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.02m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_audio_preview,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.04m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.08m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_mini_audio_preview,
                new OpenAiCost { Units = 0.00000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.01m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.02m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_realtime_preview,
                new OpenAiCost { Units = 0.000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.04m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.08m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_mini_realtime_preview,
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000003m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000024m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.01m, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.02m, Kind = KindOfCost.AudioOutput, UnitOfMeasure = UnitOfMeasure.Tokens })

            // O-Series
            .AddModel(ChatModelName.O1,
                new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000075m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O1_preview,
                new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000075m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O1_mini,
                new OpenAiCost { Units = 0.0000011m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000055m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000044m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O1_pro,
                new OpenAiCost { Units = 0.00015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O3,
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000005m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000008m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O3_pro,
                new OpenAiCost { Units = 0.00002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00008m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O3_deep_research,
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00004m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O3_mini,
                new OpenAiCost { Units = 0.0000011m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00000055m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000044m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O4_mini,
                new OpenAiCost { Units = 0.0000011m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000275m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000044m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.O4_mini_deep_research,
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000005m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000008m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Search Models
            .AddModel(ChatModelName.Gpt_4o_search_preview,
                new OpenAiCost { Units = 0.0000025m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4o_mini_search_preview,
                new OpenAiCost { Units = 0.00000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Computer Use
            .AddModel(ChatModelName.Computer_use_preview,
                new OpenAiCost { Units = 0.000003m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000012m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Codex
            .AddModel(ChatModelName.Codex_mini_latest,
                new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000000375m, Kind = KindOfCost.CachedInput, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // GPT-3.5 Series (Legacy)
            .AddModel(ChatModelName.Gpt_3_5_turbo,
                new OpenAiCost { Units = 0.0000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_3_5_turbo_0125,
                new OpenAiCost { Units = 0.0000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_3_5_turbo_1106,
                new OpenAiCost { Units = 0.000001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_3_5_turbo_0613,
                new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_3_5_turbo_instruct,
                new OpenAiCost { Units = 0.0000015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_3_5_turbo_16k_0613,
                new OpenAiCost { Units = 0.000003m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000004m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // GPT-4 Legacy
            .AddModel(ChatModelName.Chatgpt_4o_latest,
                new OpenAiCost { Units = 0.000005m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000015m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_turbo_2024_04_09,
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00003m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_0125_preview,
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00003m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_1106_preview,
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00003m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_vision_preview,
                new OpenAiCost { Units = 0.00001m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00003m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_0613,
                new OpenAiCost { Units = 0.00003m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00006m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Gpt_4_32k,
                new OpenAiCost { Units = 0.00006m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.00012m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Base Models
            .AddModel(ChatModelName.Davinci_002,
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.000002m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(ChatModelName.Babbage_002,
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens },
                new OpenAiCost { Units = 0.0000004m, Kind = KindOfCost.Output, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Embeddings
            .AddModel(EmbeddingModelName.Text_embedding_3_small,
                new OpenAiCost { Units = 0.00000002m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens })
            .AddModel(EmbeddingModelName.Text_embedding_3_large,
                new OpenAiCost { Units = 0.00000013m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Tokens })

            // Image Generation (DALL-E)
            .AddModel(ImageModelName.Dalle3,
                new OpenAiCost { Units = 0.04m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Images })

            // Audio/Speech
            .AddModel(AudioModelName.Whisper,
                new OpenAiCost { Units = 0.006m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Minutes })
            .AddModel(SpeechModelName.Tts,
                new OpenAiCost { Units = 0.015m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Characters })
            .AddModel(SpeechModelName.TtsHd,
                new OpenAiCost { Units = 0.03m, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Characters });
    }
}
