using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;

namespace Rystem.OpenAi.Completion
{
    public sealed class CompletionRequestBuilder : RequestBuilder<CompletionRequest>
    {
        private TextModelType _modelType;
        internal CompletionRequestBuilder(HttpClient client,
            OpenAiConfiguration configuration,
            string[] prompts,
            IOpenAiUtility utility)
            : base(client, configuration, () =>
            {
                return new CompletionRequest()
                {
                    Prompt = prompts.ToCorrectPrompt(),
                    ModelId = TextModelType.DavinciText3.ToModel().Id
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Davinci;
            _modelType = TextModelType.DavinciText3;
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>CompletionResult</returns>
        public ValueTask<CompletionResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Request.Stream = false;
            return Client.PostAsync<CompletionResult>(Configuration.GetUri(OpenAiType.Completion, Request.ModelId!, _forced, string.Empty), Request, Configuration, cancellationToken);
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>CostResult<CompletionResult></returns>
        public async ValueTask<CostResult<CompletionResult>> ExecuteAndCalculateCostAsync(CancellationToken cancellationToken = default)
        {
            var response = await ExecuteAsync(cancellationToken);
            return new CostResult<CompletionResult>(response, () => CalculateCost(OpenAiType.Completion, response?.Usage));
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <returns>CompletionResult</returns>
        public IAsyncEnumerable<CompletionResult> ExecuteAsStreamAsync(CancellationToken cancellationToken = default)
        {
            Request.Stream = true;
            Request.BestOf = null;
            return Client.StreamAsync<CompletionResult>(
                Configuration.GetUri(OpenAiType.Completion, Request.ModelId!, _forced, string.Empty), Request, HttpMethod.Post, Configuration,
                null,
                cancellationToken);
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <returns>CostResult<CompletionResult></returns>
        public async IAsyncEnumerable<CostResult<CompletionResult>> ExecuteAsStreamAndCalculateCostAsync(
           [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var response in ExecuteAsStreamAsync(cancellationToken))
            {
                yield return new CostResult<CompletionResult>(response, () => CalculateCost(OpenAiType.Completion, response?.Usage));
            }
        }
        /// <summary>
        /// Add further prompt to the request.
        /// </summary>
        /// <param name="prompt">Prompt</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder AddPrompt(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return this;
            if (Request.Prompt is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = prompt;
                Request.Prompt = newArray;
            }
            else if (Request.Prompt is string value && !string.IsNullOrWhiteSpace(value))
            {
                Request.Prompt = new string[2] { value, prompt };
            }
            else
            {
                Request.Prompt = prompt;
            }
            return this;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithModel(TextModelType model)
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
        public CompletionRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.ModelId = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
            return this;
        }
        /// <summary>
        /// The suffix that comes after a completion of inserted text. Defaults to null.
        /// </summary>
        /// <param name="suffix">Suffix</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithSuffix(string suffix)
        {
            Request.Suffix = suffix;
            return this;
        }
        /// <summary>
        /// How many tokens to complete to. Can return fewer if a stop sequence is hit.  Defaults to 16.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder SetMaxTokens(int value)
        {
            Request.MaxTokens = value;
            return this;
        }
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering this or Nucleus sampling but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithTemperature(double value)
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
        public CompletionRequestBuilder WithNucleusSampling(double value)
        {
            if (value < 0)
                throw new ArgumentException("Nucleus sampling with a value lesser than 0");
            if (value > 1)
                throw new ArgumentException("Nucleus sampling with a value greater than 1");
            Request.TopP = value;
            return this;
        }
        /// <summary>
        /// How many different choices to request for each prompt.  Defaults to 1.
        /// Note: Because this parameter generates many completions, it can quickly consume your token quota. Use carefully and ensure that you have reasonable settings for max_tokens and stop.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithNumberOfChoicesPerPrompt(int value)
        {
            Request.NumberOfChoicesPerPrompt = value;
            return this;
        }
        /// <summary>
        /// Include the log probabilities on the logprobs most likely tokens, as well the chosen tokens. For example, if logprobs is 5, the API will return a list of the 5 most likely tokens. The API will always return the logprob of the sampled token, so there may be up to logprobs+1 elements in the response.
        /// The maximum value for logprobs is 5. If you need more than this, please contact open api through their <see href="https://help.openai.com/en/">Help center</see> and describe your use case.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithLogProbs(int value)
        {
            Request.Logprobs = value;
            return this;
        }
        /// <summary>
        /// Echo back the prompt in addition to the completion.
        /// </summary>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithEcho()
        {
            Request.Echo = true;
            return this;
        }
        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="values">Sequences</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithStopSequence(params string[] values)
        {
            if (values.Length > 1)
                Request.StopSequence = values;
            else if (values.Length == 1)
                Request.StopSequence = values[0];
            return this;
        }
        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="value">Sequences</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder AddStopSequence(string value)
        {
            if (Request.StopSequence == null)
                Request.StopSequence = value;
            else if (Request.StopSequence is string stringableSequence)
                Request.StopSequence = new string[2] { stringableSequence, value };
            else if (Request.StopSequence is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = value;
                Request.StopSequence = newArray;
            }
            return this;
        }
        /// <summary>
        /// The scale of the penalty for how often a token is used.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithFrequencyPenalty(double value)
        {
            if (value < -1)
                throw new ArgumentException("Frequency penalty with a value lesser than -1");
            if (value > 1)
                throw new ArgumentException("Frequency penalty with a value greater than 1");
            Request.FrequencyPenalty = value;
            return this;
        }
        /// <summary>
        /// The scale of the penalty applied if a token is already present at all.  Should generally be between 0 and 1, although negative numbers are allowed to encourage token reuse.  Defaults to 0.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithPresencePenalty(double value)
        {
            if (value < -1)
                throw new ArgumentException("Presence penalty with a value lesser than -1");
            if (value > 1)
                throw new ArgumentException("Presence penalty with a value greater than 1");
            Request.PresencePenalty = value;
            return this;
        }
        /// <summary>
        /// Generates best_of completions server-side and returns the "best" (the one with the highest log probability per token). Results cannot be streamed.
        /// When used with n, best_of controls the number of candidate completions and n specifies how many to return – best_of must be greater than n.
        /// Note: Because this parameter generates many completions, it can quickly consume your token quota.Use carefully and ensure that you have reasonable settings for max_tokens and stop.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder BestOf(int value)
        {
            Request.Stream = false;
            Request.BestOf = value;
            return this;
        }
        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens (specified by their token ID in the GPT tokenizer) to an associated bias value from -100 to 100. You can use this tokenizer tool (which works for both GPT-2 and GPT-3) to convert text to token IDs. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// As an example, you can pass { "50256": -100} to prevent the <|endoftext|> token from being generated.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithBias(string key, int value)
        {
            Request.Bias ??= new Dictionary<string, int>();
            if (!Request.Bias.ContainsKey(key))
                Request.Bias.Add(key, value);
            else
                Request.Bias[key] = value;
            return this;
        }
        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens (specified by their token ID in the GPT tokenizer) to an associated bias value from -100 to 100. You can use this tokenizer tool (which works for both GPT-2 and GPT-3) to convert text to token IDs. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// As an example, you can pass { "50256": -100} to prevent the <|endoftext|> token from being generated.
        /// </summary>
        /// <param name="bias"></param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithBias(Dictionary<string, int> bias)
        {
            foreach (var c in bias)
                WithBias(c.Key, c.Value);
            return this;
        }
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithUser(string user)
        {
            Request.User = user;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var tokenizer = Utility.Tokenizer.WithTextModel(_modelType);
            var cost = Utility.Cost;
            var tokens = 0;
            if (Request.Prompt is string[] array)
            {
                foreach (var x in array)
                    tokens += tokenizer.Encode(x).NumberOfTokens;
            }
            else if (Request.Prompt is string stringable)
            {
                tokens += tokenizer.Encode(stringable).NumberOfTokens;
            }
            return cost.Configure(settings =>
            {
                settings
                    .WithFamily(_familyType)
                    .WithType(OpenAiType.Completion);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                PromptTokens = tokens
            });
        }
    }
}
