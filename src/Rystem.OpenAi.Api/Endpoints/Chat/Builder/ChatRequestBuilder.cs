using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatRequestBuilder : RequestBuilder<ChatRequest>
    {
        internal ChatRequestBuilder(HttpClient client, OpenAiConfiguration configuration, ChatMessage message) : base(client,
            configuration,
            () =>
            {
                return new ChatRequest()
                {
                    Messages = new List<ChatMessage>() { message },
                    ModelId = ChatModelType.Gpt35Turbo0301.ToModel().Id
                };
            })
        {
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ChatResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _request.Stream = false;
            return _client.PostAsync<ChatResult>(_configuration.GetUri(OpenAiType.Chat, _request.ModelId!, _forced), _request, _configuration, cancellationToken);
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <returns>Builder</returns>
        public IAsyncEnumerable<ChatResult> ExecuteAsStreamAsync(CancellationToken cancellationToken = default)
        {
            _request.Stream = true;
            return _client.StreamAsync<ChatResult>(_configuration.GetUri(OpenAiType.Chat, _request.ModelId!, _forced), _request, HttpMethod.Post, _configuration, cancellationToken);
        }
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="message">Prompt</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessage(ChatMessage message)
        {
            _request.Messages ??= new List<ChatMessage>();
            _request.Messages.Add(message);
            return this;
        }
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessage(string content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessage { Content = content, Role = role });
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithModel(ChatModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.DavinciText"/>.
        /// </summary>
        /// <param name="modelId">Override with a custom model id</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            _forced = true;
            return this;
        }
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering this or Nucleus sampling but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithTemperature(double value)
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
        public ChatRequestBuilder WithNucleusSampling(double value)
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
        public ChatRequestBuilder WithNumberOfChoicesPerPrompt(int value)
        {
            _request.NumberOfChoicesPerPrompt = value;
            return this;
        }
        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="values">Sequences</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithStopSequence(params string[] values)
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
        public ChatRequestBuilder AddStopSequence(string value)
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
        /// The maximum number of tokens allowed for the generated answer. By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder SetMaxTokens(int value)
        {
            _request.MaxTokens = value;
            return this;
        }
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// <see href="https://platform.openai.com/docs/api-reference/parameter-details"></see>
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithPresencePenalty(double value)
        {
            if (value < -2)
                throw new ArgumentException("Presence penalty with a value lesser than -2");
            if (value > 2)
                throw new ArgumentException("Presence penalty with a value greater than 2");
            _request.PresencePenalty = value;
            return this;
        }
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
        /// <see href="https://platform.openai.com/docs/api-reference/parameter-details"></see>
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithFrequencyPenalty(double value)
        {
            if (value < -2)
                throw new ArgumentException("Frequency penalty with a value lesser than -2");
            if (value > 2)
                throw new ArgumentException("Frequency penalty with a value greater than 2");
            _request.FrequencyPenalty = value;
            return this;
        }

        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens (specified by their token ID in the tokenizer) to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithBias(string key, int value)
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
        /// Accepts a json object that maps tokens (specified by their token ID in the tokenizer) to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// </summary>
        /// <param name="bias"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithBias(Dictionary<string, int> bias)
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
        public ChatRequestBuilder WithUser(string user)
        {
            _request.User = user;
            return this;
        }
    }
}
