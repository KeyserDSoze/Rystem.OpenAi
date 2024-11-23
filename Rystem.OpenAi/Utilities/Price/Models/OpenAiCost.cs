namespace Rystem.OpenAi
{
    public class OpenAiCost
    {
        public decimal Units { get; set; }
        public KindOfCost Kind { get; set; }
        public required UnitOfMeasure UnitOfMeasure { get; set; }
    }
    public enum UnitOfMeasure
    {
        Tokens,
        Images,
        Minutes,
        Characters
    }
    public enum KindOfCost
    {
        Input,
        Output,
        CachedInput,
        Training,
        AudioInput,
        AudioOutput
    }
}
