using Xunit;

namespace Rystem.OpenAi.Test
{
    public class TokenizerTests
    {
        private readonly IOpenAiUtility _openAiUtility;
        public TokenizerTests(IOpenAiUtility openAiUtility)
        {
            _openAiUtility = openAiUtility;
        }
        [Theory]
        [InlineData("This model is the perfect model for you.", 9)]
        [InlineData("Multiple models, each with different capabilities and price points. Prices are per 1000 tokens. You can think of tokens as pieces of words, where 1000 tokens is about 750 words.", 40)]
        public void Gpt4(string value, int numberOfTokens)
        {
            var encoded = _openAiUtility.Tokenizer.WithChatModel(ChatModelType.Gpt4).Encode(value);
            Assert.Equal(numberOfTokens, encoded.NumberOfTokens);
            var decoded = _openAiUtility.Tokenizer.Decode(encoded.EncodedTokens);
            Assert.Equal(value, decoded);
        }
    }
}
