using System;

namespace Rystem.OpenAi
{
    internal sealed class OpenAiUtility : IOpenAiUtility
    {
        public IOpenAiTokenizer Tokenizer { get; }
        public OpenAiUtility(IOpenAiTokenizer tokenizer)
        {
            Tokenizer = tokenizer;
        }
        public double EuclideanDistance(float[] from, float[] to)
        {
            if (from.Length != to.Length)
            {
                throw new ArgumentException("Both arrays must have the same length.");
            }
            double sumOfSquares = 0;
            for (var i = 0; i < from.Length; i++)
            {
                var difference = from[i] - to[i];
                sumOfSquares += difference * difference;
            }
            return Math.Sqrt(sumOfSquares);
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
