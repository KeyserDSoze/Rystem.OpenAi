using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Assistant
{
    public interface IOpenAiAssistant : IOpenAiBase<IOpenAiAssistant, ChatModelName>
    {
        /// <summary>
        /// The name of the assistant. The maximum length is 256 characters.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IOpenAiAssistant WithName(string name);
        /// <summary>
        /// The description of the assistant. The maximum length is 512 characters.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IOpenAiAssistant WithDescription(string description);
        /// <summary>
        /// The system instructions that the assistant uses. The maximum length is 256,000 characters.
        /// You can add more WithInstructions to append more instructions but the maximum at all is always 256,000 characters.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        IOpenAiAssistant WithInstructions(string text);
        /// <summary>
        /// Add code interpreter to the assistant.
        /// </summary>
        /// <returns></returns>
        IOpenAiAssistant WithCodeInterpreter();
        /// <summary>
        /// Add file search to the assistant.
        /// </summary>
        /// <param name="maxNumberOfResults"></param>
        /// <returns></returns>
        IOpenAiFileSearchAssistant WithFileSearch(int maxNumberOfResults = 20);
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        IOpenAiAssistant AddFunctionTool(FunctionTool tool);
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// You can add a class T as a function tool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        IOpenAiAssistant AddFunctionTool<T>(string name, string? description = null, bool? strict = null);
        /// <summary>
        /// Use this to add a list of functions the model may generate JSON inputs for. A max of 128 functions are supported.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        IOpenAiAssistant AddFunctionTool(MethodInfo function, bool? strict = null);
        /// <summary>
        /// A set of resources that are used by the assistant's tools. The resources are specific to the type of tool. For example, the code_interpreter tool requires a list of file IDs, while the file_search tool requires a list of vector store IDs.
        /// </summary>
        /// <returns></returns>
        IOpenAiToolResourcesAssistant WithToolResources();
        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IOpenAiAssistant AddMetadata(string key, string value);
        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        IOpenAiAssistant AddMetadata(Dictionary<string, string> metadata);
        /// <summary>
        /// Remove a metadata key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IOpenAiAssistant RemoveMetadata(string key);
        /// <summary>
        /// Clear all metadata.
        /// </summary>
        /// <returns></returns>
        IOpenAiAssistant ClearMetadata();
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiAssistant ForceResponseFormat(FunctionTool function);
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiAssistant ForceResponseFormat(MethodInfo function);
        /// <summary>
        /// Structured Outputs are available in our latest large language models, starting with GPT-4o:
        /// gpt-4o-mini-2024-07-18 and later
        /// gpt-4o-2024-08-06 and later
        /// </summary>
        /// <param name="function"></param>
        /// <returns>Builder</returns>
        IOpenAiAssistant ForceResponseFormat<T>();
        /// <summary>
        /// When JSON mode is turned on, the model's output is ensured to be valid JSON, except for in some edge cases that you should detect and handle appropriately.
        /// </summary>
        /// <returns></returns>
        IOpenAiAssistant ForceResponseAsJsonFormat();
        /// <summary>
        /// Classic response as text.
        /// </summary>
        /// <returns></returns>
        IOpenAiAssistant ForceResponseAsText();
        /// <summary>
        /// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering this or Nucleus sampling but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiAssistant WithTemperature(double value);
        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered. It is generally recommend to use this or temperature but not both.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        IOpenAiAssistant WithNucleusSampling(double value);
        /// <summary>
        /// Create the assistant.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<AssistantRequest> CreateAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete the assistant.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AssistantDeleteResponse</returns>
        ValueTask<AssistantDeleteResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
        /// <summary>
        /// List all the assistants.
        /// </summary>
        /// <param name="take"></param>
        /// <param name="elementId"></param>
        /// <param name="getAfterTheElementId"></param>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<AssistantListRequest> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);
        /// <summary>
        /// Retrieve the assistant.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<AssistantRequest> RetrieveAsync(string id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Update the assistant.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<AssistantRequest> UpdateAsync(string id, CancellationToken cancellationToken = default);
    }
}
