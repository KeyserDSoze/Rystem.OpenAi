using System;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiUtility : IOpenAiUtility
    {
        public IOpenAiTokenizer Tokenizer { get; }
        public IOpenAiCost Cost { get; }
        public IOpenAiExecutor Executor { get; }
        public OpenAiUtility(IOpenAiTokenizer tokenizer, IOpenAiCost cost, IOpenAiExecutor executor)
        {
            Tokenizer = tokenizer;
            Cost = cost;
            Executor = executor;
        }

        public double CosineSimilarity(float[] from, float[] to)
        {
            var value = (to.Length < from.Length) ? to.Length : from.Length;
            var sum = 0.0d;
            var powerizedFrom = 0.0d;
            var powerizedTo = 0.0d;
            for (var i = 0; i < value; i++)
            {
                sum += from[i] * to[i];
                powerizedFrom += Math.Pow(from[i], 2);
                powerizedTo += Math.Pow(to[i], 2);
            }
            return sum / (Math.Sqrt(powerizedFrom) * Math.Sqrt(powerizedTo));
        }
    }
}
