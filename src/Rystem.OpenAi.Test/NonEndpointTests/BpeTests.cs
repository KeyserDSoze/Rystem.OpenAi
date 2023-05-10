using Rystem.OpenAi.Utilities.Tokenizer;
using Xunit;

namespace Rystem.OpenAi.Test
{
    public class BpeTests
    {
        [Theory]
        [InlineData("sample", 1)]
        public void Execute(string text, int token)
        {
            var r50k = BpeInMemory.GetRight(BytePairEncodingType.R50k);
            var response = r50k.Encode(text);
            Assert.Equal(token, response.NumberOfTokens);
            var p50k = BpeInMemory.GetRight(BytePairEncodingType.P50k);
            response = p50k.Encode(text);
            Assert.Equal(token, response.NumberOfTokens);
            var p50kEdit = BpeInMemory.GetRight(BytePairEncodingType.P50k_Edit);
            response = p50kEdit.Encode(text);
            Assert.Equal(token, response.NumberOfTokens);
            var cl100k = BpeInMemory.GetRight(BytePairEncodingType.Cl100k);
            response = cl100k.Encode(text);
            Assert.Equal(token, response.NumberOfTokens);
            var encoder = BpeInMemory.GetEncoder("davinci");
            var encoded = encoder.Encode(text);
            var decoded = encoder.Decode(encoded.EncodedTokens);
            Assert.Equal(text, decoded);
        }
    }
}
