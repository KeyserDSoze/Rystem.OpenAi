using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Models;

namespace Rystem.OpenAi.Completion
{
    public sealed class CompletionRequestBuilder : RequestBuilder<CompletionRequest>
    {
        internal CompletionRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string[] prompts)
            : base(client, configuration, () =>
            {
                return new CompletionRequest()
                {
                    Prompt = prompts.Length > 1 ? (object)prompts : (prompts.Length == 1 ? prompts[1] : string.Empty),
                    ModelId = TextModelType.DavinciText3.ToModel().Id
                };
            })
        {
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<CompletionResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _request.Stream = false;
            return _client.PostAsync<CompletionResult>(_configuration.GetUri(OpenAi.Completion, _request.ModelId!), _request, cancellationToken);
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <returns>Builder</returns>
        public IAsyncEnumerable<CompletionResult> ExecuteAsStreamAsync(CancellationToken cancellationToken = default)
        {
            _request.Stream = true;
            _request.BestOf = null;
            return _client.PostStreamAsync<CompletionResult>(_configuration.GetUri(OpenAi.Completion, _request.ModelId!), _request, cancellationToken);
        }
        /// <summary>
        /// Add further prompt to the request.
        /// </summary>
        /// <param name="prompt">Prompt</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder AddPrompt(string prompt)
        {
            if (_request.Prompt is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = prompt;
                _request.Prompt = newArray;
            }
            else if (_request.Prompt is string value)
            {
                _request.Prompt = new string[2] { value, prompt };
            }
            else
            {
                _request.Prompt = prompt;
            }
            return this;
        }
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithModel(TextModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            return this;
        }
        /// <summary>
        /// The suffix that comes after a completion of inserted text. Defaults to null.
        /// </summary>
        /// <param name="suffix">Suffix</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithSuffix(string suffix)
        {
            _request.Suffix = suffix;
            return this;
        }
        /// <summary>
        /// How many tokens to complete to. Can return fewer if a stop sequence is hit.  Defaults to 16.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder SetMaxTokens(int value)
        {
            _request.MaxTokens = value;
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
            _request.Temperature = value;
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
            _request.TopP = value;
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
            _request.NumberOfChoicesPerPrompt = value;
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
            _request.Logprobs = value;
            return this;
        }
        /// <summary>
        /// Echo back the prompt in addition to the completion.
        /// </summary>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder WithEcho()
        {
            _request.Echo = true;
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
                _request.StopSequence = values;
            else if (values.Length == 1)
                _request.StopSequence = values[0];
            return this;
        }
        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="value">Sequences</param>
        /// <returns>Builder</returns>
        public CompletionRequestBuilder AddStopSequence(string value)
        {
            if (_request.StopSequence == null)
                _request.StopSequence = value;
            else if (_request.StopSequence is string stringableSequence)
                _request.StopSequence = new string[2] { stringableSequence, value };
            else if (_request.StopSequence is string[] array)
            {
                var newArray = new string[array.Length + 1];
                array.CopyTo(newArray, 0);
                newArray[^1] = value;
                _request.StopSequence = newArray;
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
            _request.FrequencyPenalty = value;
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
            _request.PresencePenalty = value;
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
            _request.Stream = false;
            _request.BestOf = value;
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
            _request.Bias ??= new Dictionary<string, int>();
            if (!_request.Bias.ContainsKey(key))
                _request.Bias.Add(key, value);
            else
                _request.Bias[key] = value;
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
            _request.User = user;
            return this;
        }
    }
}
