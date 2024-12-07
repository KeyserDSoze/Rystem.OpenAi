using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal sealed class BpeEconding
    {
        private readonly BytePairEncodingCore _bytePairEncodingCoreProcessor;
        private readonly Dictionary<string, int> _specialTokenMappings;

        private BpeEconding(string patternString,
            Dictionary<byte[], int> bytePairRanks,
            Dictionary<string, int> specialTokenMappings,
            int? explicitNVocab = null)
        {
            MaxTokenValue = Math.Max(
                GetMaxValueFromDictionary(bytePairRanks),
                GetMaxValueFromDictionary(specialTokenMappings)
            );
            _specialTokenMappings = specialTokenMappings;

            if (explicitNVocab.HasValue)
            {
                if (bytePairRanks.Count + specialTokenMappings.Count != explicitNVocab.Value)
                {
                    throw new ArgumentException(
                        "The number of mergeable tokens and special tokens must be equal to explicit_n_vocab.");
                }

                if (MaxTokenValue != explicitNVocab.Value - 1)
                {
                    throw new ArgumentException("The maximum token value must be equal to explicit_n_vocab - 1.");
                }
            }

            _bytePairEncodingCoreProcessor =
                new BytePairEncodingCore(bytePairRanks, specialTokenMappings, new Regex(patternString, RegexOptions.None, TimeSpan.FromSeconds(1)));
        }

        private int MaxTokenValue { get; }

        public static BpeEconding GetEncoding(string modelName)
        {
            var modelParams = ModelParamsGenerator.GetModelParams(modelName);

            var encoding = new BpeEconding(modelParams.Regex, modelParams.MergeableRanks,
                modelParams.SpecialTokens, modelParams.ExplicitNVocab);
            return encoding;
        }

        private static string SpecialTokenRegex(ISet<string> tokens)
        {
            var escapedTokens = new List<string>();
            foreach (var token in tokens)
            {
                escapedTokens.Add(Regex.Escape(token));
            }

            var inner = string.Join("|", escapedTokens);
            return $"({inner})";
        }

        public BytePairEncodingResponse Encode(string lineToEncode)
        {
            var specialTokensSet = new HashSet<string>(_specialTokenMappings.Keys);
            var regexPattern = SpecialTokenRegex(specialTokensSet);
            var match = Regex.Match(lineToEncode, regexPattern, RegexOptions.None, TimeSpan.FromSeconds(1));
            if (match.Success)
            {
                throw new ArgumentException($"Disallowed special token found: {match.Value}");
            }

            var encodedLine = _bytePairEncodingCoreProcessor.EncodeNative(lineToEncode, specialTokensSet);
            return encodedLine;
        }

        public string Decode(List<int> inputTokensToDecode)
        {
            var qq = _bytePairEncodingCoreProcessor.DecodeNative(inputTokensToDecode.ToArray());
            var utf8Encoding = Encoding.GetEncoding("UTF-8");
            return utf8Encoding.GetString(qq.ToArray());
        }

        private static int GetMaxValueFromDictionary(Dictionary<byte[], int> dictionary)
        {
            return dictionary.Values.Prepend(0).Max();
        }

        private static int GetMaxValueFromDictionary(Dictionary<string, int> dictionary)
        {
            return dictionary.Values.Prepend(0).Max();
        }
    }
}
