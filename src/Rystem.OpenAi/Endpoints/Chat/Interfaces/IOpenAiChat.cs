using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Chat
{
    public interface IOpenAiChat : IOpenAiBase<IOpenAiChat, ChatModelName>
    {
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>ChatResult</returns>
        ValueTask<ChatResult> ExecuteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Specifies where the results should stream and be returned at one time.
        /// </summary>
        /// <param name="withUsage">Add usage to the last chunk.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>StreamingChatResult</returns>
        IAsyncEnumerable<ChunkChatResult> ExecuteAsStreamAsync(bool withUsage = true, CancellationToken cancellationToken = default);
        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="message">Prompt</param>
        /// <returns>Builder</returns>
        IOpenAiChat AddMessage(ChatMessageRequest message);

        /// <summary>
        /// Add some messages to the request
        /// </summary>
        /// <param name="messages">Prompts</param>
        /// <returns>Builder</returns>
        IOpenAiChat AddMessages(params ChatMessageRequest[] messages);

        /// <summary>
        /// Get all messages added till now.
        /// </summary>
        /// <returns>List<ChatMessage></returns>
        List<ChatMessageRequest> GetCurrentMessages();

        /// <summary>
        /// Add a message to the request
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        IOpenAiChat AddMessage(string content, ChatRole role = ChatRole.User);

        /// <summary>
        /// Add a message with content text or image to the request
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder AddContent(ChatRole role = ChatRole.User);

        /// <summary>
        /// User message is a message used to send information.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        IOpenAiChat AddUserMessage(string content);

        /// <summary>
        /// System message is a message used to improve the response from chat API.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        IOpenAiChat AddSystemMessage(string content);
        /// <summary>
        /// Assistant message is a message used to improve the response from chat API.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        IOpenAiChat AddAssistantMessage(string content);
        /// <summary>
        /// Developer-provided instructions that the model should follow, regardless of messages sent by the user. With o1 models and newer, developer messages replace the previous system messages.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Builder</returns>
        IOpenAiChat AddDeveloperMessage(string content);
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering this or Nucleus sampling but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithTemperature(double value);
        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. It is generally recommend to use this or temperature but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithNucleusSampling(double value);
        /// <summary>
        /// How many different choices to request for each prompt. Defaults to 1.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithNumberOfChoicesPerPrompt(int value);

        /// <summary>
        /// Specifies the maximum number of tokens for the response.
        /// </summary>
        /// <param name="value">Max tokens</param>
        /// <returns>Builder</returns>
        IOpenAiChat SetMaxTokens(int value);

        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="values">Sequences</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithStopSequence(params string[] values);
        /// <summary>
        /// One or more sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        /// <param name="value">Sequences</param>
        /// <returns>Builder</returns>
        IOpenAiChat AddStopSequence(string value);
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// <see href="https://platform.openai.com/docs/api-reference/parameter-details"></see>
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithPresencePenalty(double value);
        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
        /// <see href="https://platform.openai.com/docs/api-reference/parameter-details"></see>
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithFrequencyPenalty(double value);
        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens (specified by their token ID in the tokenizer) to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Builder</returns>
        IOpenAiChat WithBias(string key, int value);
        /// <summary>
        /// Modify the likelihood of specified tokens appearing in the completion.
        /// Accepts a json object that maps tokens (specified by their token ID in the tokenizer) to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
        /// </summary>
        /// <param name="bias"></param>
        /// <returns>Builder</returns>
        IOpenAiChat WithBias(Dictionary<string, int> bias);
        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
        /// <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids"></see>
        /// </summary>
        /// <param name="user">Unique identifier</param>
        /// <returns>Builder</returns>
        IOpenAiChat WithUser(string user);
        /// <summary>
        /// This feature is in Beta. If specified, our system will make a best effort to sample deterministically, 
        /// such that repeated requests with the same seed and parameters should return the same result.
        /// Determinism is not guaranteed, and you should refer to the system_fingerprint response parameter to monitor changes in the backend.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        IOpenAiChat WithSeed(int? seed);
        /// <summary>
        /// Add a message with content text or image to the request as User
        /// </summary>
        /// <param name="content"></param>
        /// <param name="role"></param>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder AddUserContent();
        /// <summary>
        /// Add a message with content text or image to the request as System
        /// </summary>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder AddSystemContent();
        /// <summary>
        /// Add a message with content text or image to the request as Assistant
        /// </summary>
        /// <returns>Builder</returns>
        ChatMessageContentBuilder AddAssistantContent();
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiChat ForceResponseFormat(FunctionTool function);
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiChat ForceResponseFormat(MethodInfo function);
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiChat ForceResponseFormat<T>();
        /// <summary>
        /// When JSON mode is turned on, the model's output is ensured to be valid JSON, except for in some edge cases that you should detect and handle appropriately.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat ForceResponseAsJsonFormat();
        /// <summary>
        /// Classic response as text.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat ForceResponseAsText();
        /// <summary>
        /// It means the model will not call any tool and instead generates a message.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat AvoidCallingTools();
        /// <summary>
        /// It means the model must call one or more tools.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat ForceCallTools();
        /// <summary>
        /// It means the model may call one or more tools.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat CanCallTools();
        /// <summary>
        /// Remove all functions added until now.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat ClearTools();
        /// <summary>
        /// Specifying a particular tool via its name forces the model to call that tool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IOpenAiChat ForceCallFunction(string name);
        /// <summary>
        /// Remove the default behavior that generates multiple function calls in a single response, indicating that they should be called in parallel.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat AvoidParallelToolCall();
        /// <summary>
        /// Set the default behavior that generates multiple function calls in a single response, indicating that they should be called in parallel.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat ParallelToolCall();
        /// <summary>
        /// If also the Project is Scale tier enabled, the system will utilize scale tier credits until they are exhausted.
        /// Otherwise if the Project is not Scale tier enabled, the request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat WithAutoServiceTier();
        /// <summary>
        /// The request will be processed using the default service tier with a lower uptime SLA and no latency guarantee.
        /// </summary>
        /// <returns></returns>
        IOpenAiChat WithDefaultServiceTier();
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        IOpenAiChat AddFunctionTool(FunctionTool tool);
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// You can add a class T as a function tool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        IOpenAiChat AddFunctionTool<T>(string name, string? description = null, bool? strict = null);
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        IOpenAiChat AddFunctionTool(MethodInfo function, bool? strict = null);
    }
}
