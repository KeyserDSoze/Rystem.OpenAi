namespace Rystem.OpenAi
{
    public interface IOpenAiUtility
    {
        double CosineSimilarity(float[] from, float[] to);
        double EuclideanDistance(float[] from, float[] to);
        IOpenAiTokenizer Tokenizer { get; }
    }
}
