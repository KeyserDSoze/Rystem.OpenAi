using System.Collections.Generic;
using System.Linq;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal static class BpeInMemory
    {
        private static readonly BpeEconding s_r50kBaseEncoding = BpeEconding.GetEncoding("r50k_base");
        private static readonly BpeEconding s_p50kBaseEncoding = BpeEconding.GetEncoding("p50k_base");
        private static readonly BpeEconding s_p50kEditEncoding = BpeEconding.GetEncoding("p50k_edit");
        private static readonly BpeEconding s_cl100kBaseEncoding = BpeEconding.GetEncoding("cl100k_base");
        public static BpeEconding GetRight(BytePairEncodingType type)
        {
            switch (type)
            {
                case BytePairEncodingType.R50k:
                    return s_r50kBaseEncoding;
                case BytePairEncodingType.P50k:
                    return s_p50kBaseEncoding;
                case BytePairEncodingType.P50k_Edit:
                    return s_p50kEditEncoding;
                default:
                case BytePairEncodingType.Cl100k:
                    return s_cl100kBaseEncoding;
            }
        }
        public static BpeEconding GetEncoder(string? modelId)
        {
            if (modelId != null)
                foreach (var map in from map in Mapping
                                    where map.StartsWith.Any(x => modelId.StartsWith(x))
                                    select map)
                {
                    return GetRight(map.Type);
                }
            return s_cl100kBaseEncoding;
        }
        public static readonly List<BpeMapper> Mapping = new List<BpeMapper>();

        public sealed class BpeMapper
        {
            public List<string> StartsWith { get; set; }
            public BytePairEncodingType Type { get; set; }
        }

        static BpeInMemory()
        {
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.Cl100k,
                StartsWith = new List<string>
                {
                    "gpt-4", "gpt-3.5", "text-embedding-"
                }
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.P50k,
                StartsWith = new List<string>
                {
                    "text-davinci-002", "text-davinci-003", "code-davinci", "davinci-codex", "cushman-codex"
                }
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.P50k_Edit,
                StartsWith = new List<string>
                {
                    "text-davinci-edit-", "code-davinci-edit-"
                }
            });
            Mapping.Add(new BpeMapper
            {
                Type = BytePairEncodingType.R50k,
                StartsWith = new List<string>
                {
                    "text-", "davinci", "curie", "babbage", "ada", "code-search-"
                }
            });
        }
    }
}
