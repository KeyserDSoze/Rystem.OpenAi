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
        public IOpenAiTokenizer WithChatModel(ChatModelType chatModelType)
            => WithModel(chatModelType.ToModelId());
        public IOpenAiTokenizer WithTextModel(TextModelType textModelType)
            => WithModel(textModelType.ToModelId());
        public IOpenAiTokenizer WithEditModel(EditModelType editModelType)
            => WithModel(editModelType.ToModelId());
        public IOpenAiTokenizer WithEmbeddingModel(EmbeddingModelType embeddingModelType)
            => WithModel(embeddingModelType.ToModelId());
        public IOpenAiTokenizer WithModerationModel(ModerationModelType moderationModelType)
            => WithModel(moderationModelType.ToModelId());
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
