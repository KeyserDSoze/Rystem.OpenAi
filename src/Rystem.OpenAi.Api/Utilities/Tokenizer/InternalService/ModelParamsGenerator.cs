using System;
using System.Collections.Generic;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal static class ModelParamsGenerator
    {
        private const string EndOfText = "<|endoftext|>";
        private const string FimPrefix = "<|fim_prefix|>";
        private const string FimMiddle = "<|fim_middle|>";
        private const string FimSuffix = "<|fim_suffix|>";
        private const string EndOfPrompt = "<|endofprompt|>";
        private const string BasePath = "Rystem.OpenAi.Api.Utilities.Tokenizer.Files.";
        public static ModelParams GetModelParams(string name)
        {
            return name.ToLower() switch
            {
                "r50k_base" => R50KBase(),
                "p50k_base" => P50KBase(),
                "p50k_edit" => P50KEdit(),
                "cl100k_base" => Cl100KBase(),
                _ => throw new ArgumentException($"Unknown model name: {name}")
            };
        }

        private static ModelParams R50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding($"{BasePath}r50k_base.bpe");

            return new ModelParams
            (
                @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks,
                new Dictionary<string, int> { { EndOfText, 50256 } },
                50257
            );
        }

        private static ModelParams P50KBase()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding($"{BasePath}p50k_base.bpe");

            return new ModelParams
            (
                @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks,
                new Dictionary<string, int> { { EndOfText, 50256 } },
                50281
            );
        }

        private static ModelParams P50KEdit()
        {
            var mergeableRanks = EmbeddedResourceReader.LoadTokenBytePairEncoding($"{BasePath}p50k_base.bpe");

            var specialTokens = new Dictionary<string, int>
            {
                {EndOfText, 50256}, {FimPrefix, 50281}, {FimMiddle, 50282}, {FimSuffix, 50283}
            };

            return new ModelParams
            (
                regex: @"'s|'t|'re|'ve|'m|'ll|'d| ?\p{L}+| ?\p{N}+| ?[^\s\p{L}\p{N}]+|\s+(?!\S)|\s+",
                mergeableRanks: mergeableRanks,
                specialTokens: specialTokens
            );
        }

        private static ModelParams Cl100KBase()
        {
            var mergeableRanks =
                EmbeddedResourceReader.LoadTokenBytePairEncoding($"{BasePath}cl100k_base.bpe");

            var specialTokens = new Dictionary<string, int>
            {
                {EndOfText, 100257},
                {FimPrefix, 100258},
                {FimMiddle, 100259},
                {FimSuffix, 100260},
                {EndOfPrompt, 100276}
            };

            return new ModelParams
            (
                @"(?i:'s|'t|'re|'ve|'m|'ll|'d)|[^\r\n\p{L}\p{N}]?\p{L}+|\p{N}{1,3}| ?[^\s\p{L}\p{N}]+[\r\n]*|\s*[\r\n]+|\s+(?!\S)|\s+",
                mergeableRanks,
                specialTokens
            );
        }
    }
}
