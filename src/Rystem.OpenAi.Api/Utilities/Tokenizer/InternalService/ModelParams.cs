using System.Collections.Generic;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal sealed class ModelParams
    {
        public int? ExplicitNVocab { get; }
        public string PatStr { get; }
        public Dictionary<byte[], int> MergeableRanks { get; }
        public Dictionary<string, int> SpecialTokens { get; }

        public ModelParams(
            int? explicitNVocab = null,
            string patStr = null!,
            Dictionary<byte[], int> mergeableRanks = null!,
            Dictionary<string, int>? specialTokens = null)
        {
            ExplicitNVocab = explicitNVocab;
            PatStr = patStr;
            MergeableRanks = mergeableRanks;
            SpecialTokens = specialTokens ?? new Dictionary<string, int>();
        }
    }
}
