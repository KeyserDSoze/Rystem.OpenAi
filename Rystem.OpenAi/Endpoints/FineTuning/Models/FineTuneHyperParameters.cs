using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    /// <summary>
    /// The hyperparameters used for the fine-tuning job.
    /// </summary>
    public sealed class FineTuneHyperParameters
    {
        /// <summary>
        /// The number of epochs to train the model for. An epoch refers to one full cycle through the training dataset.
        /// </summary>
        [JsonPropertyName("n_epochs")]
        public int? NEpochs { get; set; }
        /// <summary>
        /// Number of examples in each batch. A larger batch size means that model parameters are updated less frequently, but with lower variance.
        /// </summary>
        [JsonPropertyName("batch_size")]
        public int? BatchSize { get; set; }
        /// <summary>
        /// Scaling factor for the learning rate. A smaller learning rate may be useful to avoid overfitting.
        /// </summary>
        [JsonPropertyName("learning_rate_multiplier")]
        public float? LearningRateMultiplier { get; set; }
    }
}
