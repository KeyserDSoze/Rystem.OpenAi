using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Chat
{
    public sealed class ChatRequestBuilder : RequestBuilder<ChatRequest>
    {
        private ChatModelType _modelType;
        private readonly IEnumerable<IOpenAiChatFunction> _functions;
        internal ChatRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            ChatMessage? message,
            IOpenAiUtility utility,
            IEnumerable<IOpenAiChatFunction> functions) : base(client,
            configuration,
            () =>
            {
                return new ChatRequest()
                {
                    Messages = message != null ? new List<ChatMessage>() { message } : new List<ChatMessage>(),
                    Model = ChatModelType.Gpt35Turbo_Snapshot.ToModel(),
                    ToolChoice = message?.ToolCalls?.Any(x => x.Type == ChatConstants.ToolType.Function) == true ? ChatConstants.ToolChoice.Auto : ChatConstants.ToolChoice.None,
                };
            }, utility)
        {
            _familyType = ModelFamilyType.Gpt3_5;
            _modelType = ChatModelType.Gpt35Turbo_Snapshot;
            _functions = functions;
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>ChatResult</returns>
        public async ValueTask<ChatResult> ExecuteAsync(bool autoExecuteFunction = false, bool requiredTool = false, CancellationToken cancellationToken = default)
        {
            Request.Stream = false;
            var isFromFunction = Request.Messages.Last().ToolCalls?.Any(x => x.Type == ChatConstants.ToolType.Function) == true;
            var response = await Client.PostAsync<ChatResult>(Configuration.GetUri(OpenAiType.Chat, Request.Model!, _forced, string.Empty), Request, Configuration, cancellationToken);
            if (isFromFunction && !autoExecuteFunction && response.Choices != null)
            {
                foreach (var choice in response.Choices)
                {
                    choice.FinishReason = ChatConstants.FinishReason.FunctionExecuted;
                }
            }
            if (autoExecuteFunction)
            {
                await foreach (var _ in AutoExecuteAsync(true, response, requiredTool, cancellationToken)) { }
            }
            return response;
        }
        private async IAsyncEnumerable<ChatResult> AutoExecuteAsync(bool directPost, ChatResult response, bool requiredTool, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (response.Choices?.Count > 0)
            {
                var numberOfChoicesPerPrompt = Request.NumberOfChoicesPerPrompt;
                for (var i = 0; i < response.Choices.Count; i++)
                {
                    var choice = response.Choices[i];
                    if (choice.Message?.ToolCalls != null)
                    {
                        foreach (var tool in choice.Message.ToolCalls.Where(x => x.Type == ChatConstants.ToolType.Function))
                        {
                            var function = tool.Function;
                            if (function != null)
                            {
                                var functionToExecute = _functions.FirstOrDefault(x => x.Name == function.Name);
                                if (functionToExecute != null && function.Arguments != null)
                                {
                                    var responseFromFunction = await functionToExecute.WrapAsync(function.Arguments.Replace("\r", string.Empty).Replace("\n", string.Empty));
                                    if (responseFromFunction != null)
                                    {
                                        AddMessage(choice.Message);
                                        AddToolMessage(tool.Id!, JsonSerializer.Serialize(responseFromFunction), requiredTool);
                                        WithNumberOfChoicesPerPrompt(1);
                                        await foreach (var responseForFunction in GetAsync())
                                        {
                                            if (response.Usage != null && responseForFunction.Usage != null)
                                            {
                                                response.Usage.PromptTokens += responseForFunction.Usage.PromptTokens;
                                                response.Usage.CompletionTokens += responseForFunction.Usage.CompletionTokens;
                                                response.Usage.TotalTokens += responseForFunction.Usage.TotalTokens;
                                            }
                                            response.Choices.RemoveAt(i);
                                            var choiceFromFunction = responseForFunction.Choices.LastOrDefault();
                                            if (choiceFromFunction != null)
                                            {
                                                choiceFromFunction.FinishReason = ChatConstants.FinishReason.FunctionAutoExecuted;
                                                response.Choices.Add(choiceFromFunction);
                                            }
                                            yield return responseForFunction;
                                        }
                                        async IAsyncEnumerable<ChatResult> GetAsync()
                                        {
                                            if (directPost)
                                            {
                                                yield return await Client.PostAsync<ChatResult>(Configuration.GetUri(OpenAiType.Chat, Request.Model!, _forced, string.Empty), Request, Configuration, cancellationToken);
                                            }
                                            else
                                                await foreach (var x in Client.StreamAsync<ChatResult>(Configuration.GetUri(OpenAiType.Chat, Request.Model!, _forced, string.Empty), Request, HttpMethod.Post, Configuration, null, cancellationToken))
                                                    yield return x;
                                        }
                                    }
                                    else
                                    {
                                        var lastChoice = response.Choices.LastOrDefault();
                                        if (lastChoice != null)
                                            lastChoice.FinishReason = ChatConstants.FinishReason.Null;
                                    }
                                }
                            }
                        }
                    }
                }
                WithNumberOfChoicesPerPrompt(numberOfChoicesPerPrompt ?? 1);
            }
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>CostResult<ChatResult></returns>
        public async ValueTask<CostResult<ChatResult>> ExecuteAndCalculateCostAsync(bool autoExecuteFunction = false, bool requiredTool = false, CancellationToken cancellationToken = default)
        {
            var response = await ExecuteAsync(autoExecuteFunction, requiredTool, cancellationToken);
            return new CostResult<ChatResult>(response, () => CalculateCost(OpenAiType.Chat, response?.Usage));
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <param name="autoExecuteFunction">Execute functions you injected autonomously.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>StreamingChatResult</returns>
        public async IAsyncEnumerable<StreamingChatResult> ExecuteAsStreamAsync(bool autoExecuteFunction = false,
            bool requiredTool = false,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Request.Stream = true;
            var results = new StreamingChatResult
            {
                Chunks = new List<ChatResult>()
            };
            var chatRole = ChatRole.Assistant;
            var index = -1;
            var isFromFunction = Request.Messages.Last().ToolCalls?.Any(x => x.Type == ChatConstants.ToolType.Function) == true;
            var functionCommand = autoExecuteFunction ? ChatConstants.FinishReason.FunctionAutoExecuted : ChatConstants.FinishReason.FunctionExecuted;
            var stopCommand = isFromFunction ? functionCommand : ChatConstants.FinishReason.Stop;
            await foreach (var result in Client.StreamAsync<ChatResult>(Configuration.GetUri(OpenAiType.Chat, Request.Model!, _forced, string.Empty), Request, HttpMethod.Post, Configuration, null, cancellationToken))
            {
                if (result?.Choices != null && result.Choices.Count > 0 && result.Choices[0].Delta != null)
                {
                    var currentIndex = result.Choices[0].Index;
                    if (index != currentIndex)
                    {
                        chatRole = result.Choices[0].Delta!.Role;
                        index = currentIndex;
                    }
                    else
                        result.Choices[0].Delta!.Role = chatRole;
                    results.Chunks.Add(result);
                    var lastChoice = results.Chunks[^1].Choices.LastOrDefault();
                    if (lastChoice?.FinishReason == ChatConstants.FinishReason.Stop)
                    {
                        lastChoice.FinishReason = stopCommand;
                    }
                    yield return results;
                }
            }
            if (autoExecuteFunction)
            {
                if (results.Chunks.Any(x => x.Choices.Any(t => t.Delta?.ToolCalls?.Any(q => q.Type == ChatConstants.ToolType.Function) == true)))
                {
                    var functionName = string.Empty;
                    var functionArgument = new StringBuilder();
                    var functionId = string.Empty;
                    for (var i = 0; i < results.Chunks.Count; i++)
                    {
                        var chunk = results.Chunks[i];
                        if (chunk.Choices != null)
                        {
                            for (var j = 0; j < chunk.Choices.Count; j++)
                            {
                                var choice = chunk.Choices[j];
                                var currentChoice = choice.Delta?.ToolCalls?.FirstOrDefault();
                                if (currentChoice?.Type == ChatConstants.ToolType.Function)
                                {
                                    if (!string.IsNullOrWhiteSpace(functionName))
                                    {

                                    }
                                    functionName = currentChoice.Function!.Name;
                                    functionArgument.Clear();
                                    functionId = currentChoice?.Id;
                                }
                                else if (!string.IsNullOrWhiteSpace(currentChoice?.Function?.Arguments))
                                {
                                    functionArgument.AppendLine(currentChoice?.Function?.Arguments);
                                }
                            }
                        }
                    }
                    await foreach (var x in AutoExecuteAsync(false, new ChatResult
                    {
                        Choices = new List<ChatChoice>
                           {
                              new ChatChoice{
                                  Index = 0,
                                  Message = new ChatMessage
                                  {
                                      Role = ChatRole.Assistant,
                                      ToolCallId = functionId,
                                      ToolCalls = new List<ChatMessageTool>
                                      {
                                          new ChatMessageTool
                                          {
                                              Id = functionId,
                                              Type = ChatConstants.ToolType.Function,
                                              Index = 0,
                                              Function = new ChatMessageFunctionResponse
                                              {
                                                  Name = functionName,
                                                  Arguments = functionArgument.ToString()
                                              }
                                          }
                                      }
                                  }
                              },
                        }
                    },
                        requiredTool,
                        cancellationToken))
                    {
                        results.Chunks.Add(x);
                        yield return results;
                    }
                }
            }
        }
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <returns>CostResult<ChatResult></returns>
        public async IAsyncEnumerable<CostResult<StreamingChatResult>> ExecuteAsStreamAndCalculateCostAsync(
            bool autoExecuteFunction = false,
            bool requiredTool = false,
           [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var response in ExecuteAsStreamAsync(autoExecuteFunction, requiredTool, cancellationToken))
            {
                yield return new CostResult<StreamingChatResult>(response, () => CalculateCost(OpenAiType.Chat, response?.LastChunk?.Usage));
            }
        }
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="message">Prompt</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessage(ChatMessage message)
        {
            Request.Messages ??= new List<ChatMessage>();
            Request.Messages.Add(message);
            return this;
        }
        /// <summary>
        /// Add some messages to the request
        /// </summary>
        /// <param name="messages">Prompts</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessages(params ChatMessage[] messages)
        {
            Request.Messages ??= new List<ChatMessage>();
            foreach (var message in messages)
                Request.Messages.Add(message);
            return this;
        }
        /// <summary>
        /// Get all messages added till now.
        /// </summary>
        /// <returns>List<ChatMessage></returns>
        public List<ChatMessage> GetCurrentMessages()
            => Request.Messages ?? new List<ChatMessage>();
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessage(string content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessage { Content = content, Role = role });
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddMessage(ChatMessageContent content, ChatRole role = ChatRole.User)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = role });
        /// <summary>
        /// Add a message with content text or image to the request
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        public ChatMessageContentBuilder AddContent(ChatRole role = ChatRole.User)
            => new ChatMessageContentBuilder(this, role);
        /// <summary>
        /// User message is a message used to send information.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddUserMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.User });
        /// <summary>
        /// User message is a message used to send information.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddUserMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.User });
        /// <summary>
        /// Add a message with content text or image to the request as User
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        public ChatMessageContentBuilder AddUserContent()
            => new ChatMessageContentBuilder(this, ChatRole.User);
        /// <summary>
        /// System message is a message used to improve the response from chat api.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddSystemMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.System });
        /// <summary>
        /// System message is a message used to improve the response from chat api.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddSystemMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.System });
        /// <summary>
        /// Add a message with content text or image to the request as System
        /// </summary>
        /// <returns>Builder</returns>
        public ChatMessageContentBuilder AddSystemContent()
            => new ChatMessageContentBuilder(this, ChatRole.System);
        /// <summary>
        /// Assistant message is the response from chat api, usually you don't need to set this message.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddAssistantMessage(string content)
            => AddMessage(new ChatMessage { Content = content, Role = ChatRole.Assistant });
        /// <summary>
        /// Assistant message is the response from chat api, usually you don't need to set this message.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddAssistantMessage(ChatMessageContent content)
            => AddMessage(new ChatMessage { Content = new List<ChatMessageContent> { content }, Role = ChatRole.Assistant });
        /// <summary>
        /// Add a message with content text or image to the request as Assistant
        /// </summary>
        /// <returns>Builder</returns>
        public ChatMessageContentBuilder AddAssistantContent()
            => new ChatMessageContentBuilder(this, ChatRole.Assistant);
        /// <summary>
        /// Function message is the response from external api, you need to set this message after a function is requested to call by Open Ai.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder AddToolMessage(string id, string content, bool required = false)
        {
            Request.ToolChoice = required ? ChatConstants.ToolChoice.Required : ChatConstants.ToolChoice.Auto;
            return AddMessage(new ChatMessage
            {
                Content = content,
                ToolCallId = id,
                Role = ChatRole.Tool,
            });
        }
        /// <summary>
        /// Function message is the response from external api, you need to set this message after a function is requested to call by Open Ai.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder ForceToolMessage(string id, string content)
            => AddToolMessage(id, content, true);
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithModel(ChatModelType model)
        {
            Request.Model = model.ToModel();
            _forced = false;
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
        public ChatRequestBuilder WithModel(string modelId, ModelFamilyType? basedOnFamily = null)
        {
            Request.Model = modelId;
            _forced = true;
            if (basedOnFamily != null)
                _familyType = basedOnFamily.Value;
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
            Request.Temperature = value;
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
            Request.TopP = value;
            return this;
        }
        /// <summary>
        /// How many different choices to request for each prompt. Defaults to 1.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithNumberOfChoicesPerPrompt(int value)
        {
            Request.NumberOfChoicesPerPrompt = value;
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
        public ChatRequestBuilder AddStopSequence(string value)
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
        /// The maximum number of tokens allowed for the generated answer. By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder SetMaxTokens(int value)
        {
            Request.MaxCompletionsToken = value;
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
            Request.PresencePenalty = value;
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
            Request.FrequencyPenalty = value;
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
            if (value > 100)
                throw new ArgumentException("Value of bias is greater than 100. Accepted values have range [-100, 100].");
            if (value < -100)
                throw new ArgumentException("Value of bias is lesser than -100. Accepted values have range [-100, 100].");
            Request.Bias ??= new Dictionary<string, int>();
            if (!Request.Bias.ContainsKey(key))
                Request.Bias.Add(key, value);
            else
                Request.Bias[key] = value;
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
            Request.User = user;
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var tokenizer = Utility.Tokenizer.WithChatModel(_modelType);
            var promptTokens = 3;
            if (Request?.Messages != null)
                foreach (var message in Request.Messages)
                {
                    if (message.Content is ChatMessageContent content)
                    {
                        if (content.Text != null)
                            promptTokens += tokenizer.Encode(content.Text).NumberOfTokens + 5;
                        else if (content.Image != null)
                        {
                            promptTokens += content.Image.Detail == ChatConstants.ResolutionVision.High ? 129 : 65;
                        }
                    }
                    else if (message.Content is string contentAsStrig)
                        promptTokens += tokenizer.Encode(contentAsStrig).NumberOfTokens + 5;
                }
            return CalculateCost(OpenAiType.Chat, new CompletionUsage
            {
                PromptTokens = promptTokens
            });
        }
        /// <summary>
        /// Developers can describe functions to gpt-4-snapshot and gpt-3.5-turbo-snapshot, and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.
        /// These models have been fine-tuned to both detect when a function needs to be called(depending on the user’s input) and to respond with JSON that adheres to the function signature.Function calling allows developers to more reliably get structured data back from the model.
        /// </summary>
        /// <param name="chatFunction"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithFunction(JsonFunction chatFunction)
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= new List<object>();
            Request.Tools.Add(new JsonFunctionWrapper
            {
                Function = chatFunction
            });
            return this;
        }
        /// <summary>
        /// Developers can describe functions to gpt-4-snapshot and gpt-3.5-turbo-snapshot, and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.
        /// These models have been fine-tuned to both detect when a function needs to be called(depending on the user’s input) and to respond with JSON that adheres to the function signature.Function calling allows developers to more reliably get structured data back from the model.
        /// </summary>
        /// <param name="chatFunction"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithFunction(string name)
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= new List<object>();
            var function = _functions.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"Function {name} not found. Please install with AddOpenAiChatFunction.");
            if (!Request.Tools.Any(x => x is JsonFunctionWrapper t && t.Function?.Name == name))
                Request.Tools.Add(JsonFunctionContainerManager.Instance.GetFunction(function));
            return this;
        }
        /// <summary>
        /// Developers can describe functions to gpt-4-snapshot and gpt-3.5-turbo-snapshot, and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.
        /// These models have been fine-tuned to both detect when a function needs to be called(depending on the user’s input) and to respond with JSON that adheres to the function signature.Function calling allows developers to more reliably get structured data back from the model.
        /// </summary>
        /// <param name="chatFunction"></param>
        /// <returns>Builder</returns>
        public ChatRequestBuilder IfTrueWithFunction(string name, bool condition)
        {
            if (condition)
            {
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
                Request.Tools ??= new List<object>();
                var function = _functions.FirstOrDefault(x => x.Name == name) ?? throw new ArgumentException($"Function {name} not found. Please install with AddOpenAiChatFunction.");
                if (!Request.Tools.Any(x => x is JsonFunctionWrapper t && t.Function?.Name == name))
                    Request.Tools.Add(JsonFunctionContainerManager.Instance.GetFunction(function));
            }
            return this;
        }
        /// <summary>
        /// Developers can describe functions to gpt-4-snapshot and gpt-3.5-turbo-snapshot, and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.
        /// These models have been fine-tuned to both detect when a function needs to be called(depending on the user’s input) and to respond with JSON that adheres to the function signature.Function calling allows developers to more reliably get structured data back from the model.
        /// </summary>
        /// <returns>Builder</returns>
        public ChatRequestBuilder WithAllFunctions()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= new List<object>();
            foreach (var function in _functions)
            {
                if (!Request.Tools.Any(x => x is JsonFunctionWrapper t && t.Function?.Name == function.Name))
                    Request.Tools.Add(JsonFunctionContainerManager.Instance.GetFunction(function));
            }
            return this;
        }
        /// <summary>
        /// This feature is in Beta. If specified, our system will make a best effort to sample deterministically, 
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter to monitor changes in the backend.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public ChatRequestBuilder WithSeed(int? seed)
        {
            Request.Seed = seed;
            return this;
        }
    }
}
