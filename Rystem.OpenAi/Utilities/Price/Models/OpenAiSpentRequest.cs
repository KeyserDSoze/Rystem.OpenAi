namespace Rystem.OpenAi
{
    public sealed class OpenAiSpentRequest
    {
        public decimal Input { get; set; }
        public decimal CachedInput { get; set; }
        public decimal Output { get; set; }
    }
}
