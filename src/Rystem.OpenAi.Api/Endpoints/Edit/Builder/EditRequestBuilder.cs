using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi;

namespace Rystem.OpenAi.Edit
{
    public sealed class EditRequestBuilder : RequestBuilder<EditRequest>
    {
        internal EditRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string instruction) :
            base(client, configuration, () =>
            {
                return new EditRequest
                {
                    Instruction = instruction,
                    ModelId = EditModelType.TextDavinciEdit.ToModel().Id
                };
            })
        {
        }
        /// <summary>
        /// Creates a new edit for the provided input, instruction, and parameters.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<EditResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => _client.PostAsync<EditResult>(_configuration.GetUri(OpenAi.Edit, _request.ModelId!, _forced), _request, cancellationToken);
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithModel(EditModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            _forced = true;
            return this;
        }
        /// <summary>
        /// The input text to use as a starting point for the edit.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public EditRequestBuilder SetInput(string input)
        {
            _request.Input = input;
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
            _request.Temperature = value;
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
            _request.TopP = value;
            return this;
        }
        /// <summary>
        /// How many different choices to request for each prompt. Defaults to 1.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public EditRequestBuilder WithNumberOfChoicesPerPrompt(int value)
        {
            _request.NumberOfChoicesPerPrompt = value;
            return this;
        }
    }
}
