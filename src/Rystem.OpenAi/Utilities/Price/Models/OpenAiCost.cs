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
        AudioOutput,
        ImageStandard256,
        ImageStandard512,
        ImageStandard1024,
        ImageStandard1024x1792,
        ImageStandard1792x1024,
        ImageHd1024,
        ImageHd1024x1792,
        ImageHd1792x1024,
    }
}
