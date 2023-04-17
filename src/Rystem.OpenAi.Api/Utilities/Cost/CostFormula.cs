namespace Rystem.OpenAi
{
    internal sealed class CostFormula
    {
        public decimal Training { get; set; }
        public decimal Usage { get; set; }
        public decimal PerMinute { get; set; }
        public decimal PerUnit { get; set; }
        public decimal PromptUsage { get; set; }
        public decimal CompletionUsage { get; set; }
        private const int PriceForXTokens = 1_000;
        public decimal Calculate(int trainingTokens, int usageTokens, int minutes, int units, int promptTokens, int completionTokens)
            => trainingTokens * Training / PriceForXTokens
            + usageTokens * Usage / PriceForXTokens
            + minutes * PerMinute
            + units * PerUnit
            + promptTokens * PromptUsage / PriceForXTokens
            + completionTokens * CompletionUsage / PriceForXTokens;
    }
}
