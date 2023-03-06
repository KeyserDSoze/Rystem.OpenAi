using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Models;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneRequestBuilder : RequestBuilder<FineTuneRequest>
    {
        public FineTuneRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string trainingFileId)
            : base(client, configuration, () =>
            {
                return new FineTuneRequest
                {
                    TrainingFile = trainingFileId,
                };
            })
        {
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<FineTuneResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => _client.PostAsync<FineTuneResult>(_configuration.GetUri(OpenAi.FineTune, _request.TrainingFile!), _request, cancellationToken);
        /// <summary>
        /// The ID of an uploaded file that contains validation data.
        /// If you provide this file, the data is used to generate validation metrics periodically during fine-tuning. These metrics can be viewed in the <see href="https://platform.openai.com/docs/guides/fine-tuning/analyzing-your-fine-tuned-model">fine-tuning results file</see>. Your train and validation data should be mutually exclusive.
        /// Your dataset must be formatted as a JSONL file, where each validation example is a JSON object with the keys "prompt" and "completion". Additionally, you must upload your file with the purpose fine-tune.
        /// See the <see href="https://platform.openai.com/docs/guides/fine-tuning/creating-training-data">fine-tuning guide</see> for more details.
        /// </summary>
        /// <param name="validationFileId"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithValidationFile(string validationFileId)
        {
            _request.ValidationFile = validationFileId;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public FineTuneRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            return this;
        }
        /// <summary>
        /// The number of epochs to train the model for. An epoch refers to one full cycle through the training dataset.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithNumberOfEpochs(int value)
        {
            _request.NEpochs = value;
            return this;
        }
        /// <summary>
        /// The batch size to use for training. The batch size is the number of training examples used to train a single forward and backward pass.
        /// By default, the batch size will be dynamically configured to be ~0.2% of the number of examples in the training set, capped at 256 - in general, we've found that larger batch sizes tend to work better for larger datasets.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithBatchSize(int size)
        {
            _request.BatchSize = size;
            return this;
        }
        /// <summary>
        /// The learning rate multiplier to use for training. The fine-tuning learning rate is the original learning rate used for pretraining multiplied by this value.
        /// By default, the learning rate multiplier is the 0.05, 0.1, or 0.2 depending on final batch_size (larger learning rates tend to perform better with larger batch sizes). We recommend experimenting with values in the range 0.02 to 0.2 to see what produces the best results.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithLearningRateMultiplier(double value)
        {
            _request.LearningRateMultiplier = value;
            return this;
        }
        /// <summary>
        /// The weight to use for loss on the prompt tokens. This controls how much the model tries to learn to generate the prompt (as compared to the completion which always has a weight of 1.0), and can add a stabilizing effect to training when completions are short.
        /// If prompts are extremely long (relative to completions), it may make sense to reduce this weight so as to avoid over-prioritizing learning the prompt.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithPromptLossWeight(double value)
        {
            _request.PromptLossWeight = value;
            return this;
        }
        /// <summary>
        /// If set, we calculate classification-specific metrics such as accuracy and F-1 score using the validation set at the end of every epoch. These metrics can be viewed in the <see href="https://platform.openai.com/docs/guides/fine-tuning/analyzing-your-fine-tuned-model">results file</see>.
        /// In order to compute classification metrics, you must provide a validation_file. Additionally, you must specify classification_n_classes for multiclass classification or classification_positive_class for binary classification.
        /// </summary>
        /// <returns></returns>
        public FineTuneRequestBuilder WithComputeClassificationMetrics()
        {
            _request.ComputeClassificationMetrics = true;
            return this;
        }
        /// <summary>
        /// The positive class in binary classification.
        /// This parameter is needed to generate precision, recall, and F1 metrics when doing binary classification.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithClassificationPositiveClass(string value)
        {
            _request.ClassificationPositiveClass = value;
            return this;
        }
        /// <summary>
        /// If this is provided, we calculate F-beta scores at the specified beta values. The F-beta score is a generalization of F-1 score. This is only used for binary classification.
        /// With a beta of 1 (i.e. the F-1 score), precision and recall are given the same weight. A larger beta score puts more weight on recall and less on precision. A smaller beta score puts more weight on precision and less on recall.
        /// </summary>
        /// <param name="betas"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithClassificationBetas(params string[] betas)
        {
            _request.ClassificationBetas = betas;
            return this;
        }
        /// <summary>
        /// A string of up to 40 characters that will be added to your fine-tuned model name.
        /// For example, a suffix of "custom-model-name" would produce a model name like ada:ft-your-org:custom-model-name-2022-02-15-04-21-04.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithSuffix(string value)
        {
            _request.Suffix = value;
            return this;
        }
    }
}
