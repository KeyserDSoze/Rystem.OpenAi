using System.Collections.Generic;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal sealed class ModelParams
    {
        public int? ExplicitNVocab { get; }
        public string Regex { get; }
        public Dictionary<byte[], int> MergeableRanks { get; }
        public Dictionary<string, int> SpecialTokens { get; }

        public ModelParams(
            string regex,
            Dictionary<byte[], int> mergeableRanks,
            Dictionary<string, int> specialTokens,
            int? explicitNVocab = null)
        {
            ExplicitNVocab = explicitNVocab;
            Regex = regex;
            MergeableRanks = mergeableRanks;
            SpecialTokens = specialTokens;
        }
    }
}
