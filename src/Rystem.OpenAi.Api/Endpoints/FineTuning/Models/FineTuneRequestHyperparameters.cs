using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneRequestHyperparameters
    {
        [JsonPropertyName("n_epochs")]
        public int NEpochs { get; set; }
    }
}
