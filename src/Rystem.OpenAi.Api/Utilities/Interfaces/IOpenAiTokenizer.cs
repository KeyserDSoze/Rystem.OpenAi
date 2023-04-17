using System.Collections.Generic;
using Rystem.OpenAi.Utilities.Tokenizer;

namespace Rystem.OpenAi
{
    public interface IOpenAiTokenizer
    {
        IOpenAiTokenizer WithModel(string modelId);
        IOpenAiTokenizer WithChatModel(ChatModelType chatModelType);
        IOpenAiTokenizer WithTextModel(TextModelType textModelType);
        IOpenAiTokenizer WithEditModel(EditModelType editModelType);
        IOpenAiTokenizer WithEmbeddingModel(EmbeddingModelType embeddingModelType);
        IOpenAiTokenizer WithModerationModel(ModerationModelType moderationModelType);
        string Decode(List<int> tokens);
        string Decode(BytePairEncodingType type, List<int> tokens);
        BytePairEncodingResponse Encode(string? text);
        BytePairEncodingResponse Encode(BytePairEncodingType type, string? text);
    }
}
