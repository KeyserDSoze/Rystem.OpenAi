using System.Collections.Generic;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    public sealed class BytePairEncodingResponse
    {
        public List<int>? EncodedTokens { get; set; }
        public List<string>? Tokens { get; set; }
        public int LastTokenLength { get; set; }
        public int NumberOfTokens => EncodedTokens?.Count ?? 0;
        public static BytePairEncodingResponse Default { get; } = new BytePairEncodingResponse()
        {
            EncodedTokens = [],
            Tokens = [],
            LastTokenLength = 0
        };
    }
}
