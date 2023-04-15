namespace Rystem.OpenAi
{
    public interface IOpenAiUtility
    {
        double CosineSimilarity(float[] from, float[] to);
        IOpenAiTokenizer Tokenizer { get; }
    }
}
