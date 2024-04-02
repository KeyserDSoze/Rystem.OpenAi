using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneRequestBuilder : RequestBuilder<FineTuneRequest>
    {
        internal FineTuneRequestBuilder(HttpClient client,
            OpenAiConfiguration configuration,
            string trainingFileId,
            IOpenAiUtility utility)
            : base(client, configuration, () =>
            {
                return new FineTuneRequest
                {
                    TrainingFile = trainingFileId,
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Ada;
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<FineTuneResult> ExecuteAsync(ModelFamilyType? basedOn = null, CancellationToken cancellationToken = default)
        {
            if (basedOn != null)
                _familyType = basedOn.Value;
            return Client.PostAsync<FineTuneResult>(Configuration.GetUri(OpenAiType.FineTuning, Request.TrainingFile!, _forced, string.Empty), Request, Configuration, cancellationToken);
        }

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
            Request.ValidationFile = validationFileId;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.ListAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <param name="basedOnFamily">Family of your custom model</param>
        /// <returns>Builder</returns>
        public FineTuneRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.ModelId = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
            return this;
        }
        /// <summary>
        /// The number of epochs to train the model for. An epoch refers to one full cycle through the training dataset.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public FineTuneRequestBuilder WithNumberOfEpochs(int value)
        {
            if (Request.Hyperparameters == null)
                Request.Hyperparameters = new FineTuneRequestHyperparameters();
            Request.Hyperparameters.NEpochs = value;
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
            Request.Suffix = value;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <param name="forTraining">True calculate cost for tokens during training, with False calculate the cost for usage.</param>
        /// <param name="promptTokens">Number of tokens during training/usage.</param>
        /// <returns></returns>
        public decimal CalculateCost(bool forTraining, int promptTokens)
        {
            return Utility.Cost.Configure(setup =>
            {
                setup
                    .WithFamily(_familyType)
                    .WithType(OpenAiType.FineTuning);
                if (forTraining)
                    setup
                        .ForTraining();
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                PromptTokens = promptTokens
            });
        }
    }
}
