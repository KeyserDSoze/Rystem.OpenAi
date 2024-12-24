using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal static class BpeInMemory
    {
        private static readonly BpeEncoding s_r50kBaseEncoding = BpeEncoding.GetEncoding("r50k_base");
        private static readonly BpeEncoding s_p50kBaseEncoding = BpeEncoding.GetEncoding("p50k_base");
        private static readonly BpeEncoding s_p50kEditEncoding = BpeEncoding.GetEncoding("p50k_edit");
        private static readonly BpeEncoding s_cl100kBaseEncoding = BpeEncoding.GetEncoding("cl100k_base");
        public static BpeEncoding GetRight(BytePairEncodingType type)
        {
            return type switch
            {
                BytePairEncodingType.R50k => s_r50kBaseEncoding,
                BytePairEncodingType.P50k => s_p50kBaseEncoding,
                BytePairEncodingType.P50k_Edit => s_p50kEditEncoding,
                _ => s_cl100kBaseEncoding,
            };
        }
        public static BpeEncoding GetEncoder(string? modelId)
        {
            if (modelId != null)
            {
                var finalMap = (from map in Mapping
                                where map.StartsWith?.Any(x => modelId.StartsWith(x)) == true
                                select map).FirstOrDefault();
                if (finalMap != null)
                    return GetRight(finalMap.Type);
            }
            return s_cl100kBaseEncoding;
        }
        public static readonly List<BpeMapper> Mapping = [];

        public sealed class BpeMapper
        {
            public List<string>? StartsWith { get; set; }
            public BytePairEncodingType Type { get; set; }
        }

        static BpeInMemory()
        {
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.Cl100k,
                StartsWith =
                [
                    "gpt-4", "gpt-3.5", "text-embedding-"
                ]
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.P50k,
                StartsWith =
                [
                    "text-davinci-002", "text-davinci-003", "code-davinci", "davinci-codex", "cushman-codex"
                ]
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.P50k_Edit,
                StartsWith =
                [
                    "text-davinci-edit-", "code-davinci-edit-"
                ]
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.R50k,
                StartsWith =
                [
                    "text-", "davinci", "curie", "babbage", "ada", "code-search-"
                ]
            });
        }
    }
}
