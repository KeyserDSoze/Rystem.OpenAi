using System.Collections.Generic;
using Rystem.OpenAi.Utilities.Tokenizer;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiTokenizer : IOpenAiTokenizer
    {
        private BpeEconding _encoder = BpeInMemory.GetEncoder(null);
        public IOpenAiTokenizer WithModel(string modelId)
        {
            _encoder = BpeInMemory.GetEncoder(modelId);
            return this;
        }
        public string Decode(List<int> tokens)
            => _encoder.Decode(tokens);
        public string Decode(BytePairEncodingType type, List<int> tokens)
            => BpeInMemory.GetRight(type).Decode(tokens);
        public BytePairEncodingResponse Encode(string? text)
            => text != null ? _encoder.Encode(text) : BytePairEncodingResponse.Default;
        public BytePairEncodingResponse Encode(BytePairEncodingType type, string? text)
            => text != null ? BpeInMemory.GetRight(type).Encode(text) : BytePairEncodingResponse.Default;
    }
}
