using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Completion;

namespace Rystem.OpenAi.Edit
{
    public sealed class EditRequestBuilder : RequestBuilder<EditRequest>
    {
        private EditModelType _modelType;
        internal EditRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string instruction, IOpenAiUtility utility) :
            base(client, configuration, () =>
            {
                return new EditRequest
                {
                    Instruction = instruction,
                    ModelId = EditModelType.TextDavinciEdit.ToModel().Id
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Davinci;
            _modelType = EditModelType.TextDavinciEdit;
        }
        /// <summary>
        /// Creates a new edit for the provided input, instruction, and parameters.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<EditResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => Client.PostAsync<EditResult>(Configuration.GetUri(OpenAiType.Edit, Request.ModelId!, _forced, string.Empty), Request, Configuration, cancellationToken);
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>CostResult<EditResult></returns>
        public async ValueTask<CostResult<EditResult>> ExecuteAndCalculateCostAsync(CancellationToken cancellationToken = default)
        {
            var response = await ExecuteAsync(cancellationToken);
            return new CostResult<EditResult>(response, () => CalculateCost(OpenAiType.Edit, response?.Usage));
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithModel(EditModelType model)
        {
            Request.ModelId = model.ToModel().Id;
            _familyType = model.ToFamily();
            _modelType = model;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.ListAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <param name="basedOnFamily">Family of your custom model</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.ModelId = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
            return this;
        }
        /// <summary>
        /// The input text to use as a starting point for the edit.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public EditRequestBuilder SetInput(string input)
        {
            Request.Input = input;
            return this;
        }
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering this or Nucleus sampling but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithTemperature(double value)
        {
            if (value < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (value > 2)
                throw new ArgumentException("Temperature with a value greater than 2");
            Request.Temperature = value;
            return this;
        }
        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. It is generally recommend to use this or temperature but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithNucleusSampling(double value)
        {
            if (value < 0)
                throw new ArgumentException("Nucleus sampling with a value lesser than 0");
            if (value > 1)
                throw new ArgumentException("Nucleus sampling with a value greater than 1");
            Request.TopP = value;
            return this;
        }
        /// <summary>
        /// How many different choices to request for each prompt. Defaults to 1.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithNumberOfChoicesPerPrompt(int value)
        {
            Request.NumberOfChoicesPerPrompt = value;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var tokenizer = Utility.Tokenizer.WithEditModel(_modelType);
            var promptTokens = tokenizer.Encode(Request.Instruction).NumberOfTokens + 11;
            if (Request?.Input != null)
                promptTokens += tokenizer.Encode(Request.Input).NumberOfTokens + 1;
            return CalculateCost(OpenAiType.Edit, new CompletionUsage
            {
                PromptTokens = promptTokens
            });
        }
    }
}
