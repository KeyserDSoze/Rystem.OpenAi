using Rystem.OpenAi.Chat;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Rystem.OpenAi.Assistant
{
    /// <summary>
    /// Interface for OpenAI Run interactions, providing methods to configure, execute, and manage runs within OpenAI threads.
    /// </summary>
    public interface IOpenAiRun : IOpenAiBase<IOpenAiRun, ChatModelName>
    {
        /// <summary>
        /// Associates a thread with the run by thread ID.
        /// </summary>
        /// <param name="threadId">The ID of the thread to associate with the run.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun WithThread(string threadId);

        /// <summary>
        /// Provides a helper to configure thread details for the run.
        /// </summary>
        /// <returns>A helper object for thread configuration.</returns>
        ThreadHelper<IOpenAiRun> WithThread();

        /// <summary>
        /// Includes file search context in the run configuration.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun IncludeFileSearchContext();

        /// <summary>
        /// Excludes file search context from the run configuration.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ExcludeFileSearchContext();

        /// <summary>
        /// Configures the run to avoid calling tools.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun AvoidCallingTools();

        /// <summary>
        /// Forces the run to call tools.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceCallTools();

        /// <summary>
        /// Allows the run to automatically determine whether tools should be called.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun CanCallTools();

        /// <summary>
        /// Forces the run to call a specific function by name.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceCallFunction(string name);

        /// <summary>
        /// Configures the run to avoid parallel tool calls.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun AvoidParallelToolCall();

        /// <summary>
        /// Configures the run to allow parallel tool calls.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ParallelToolCall();

        /// <summary>
        /// Sets the maximum number of tokens for the run's prompt.
        /// </summary>
        /// <param name="value">The maximum number of tokens.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun SetMaxPromptTokens(int value);

        /// <summary>
        /// Sets the maximum number of tokens for the run's completion.
        /// </summary>
        /// <param name="value">The maximum number of tokens.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun SetMaxCompletionTokens(int value);

        /// <summary>
        /// Forces the response format to use a specific function tool.
        /// </summary>
        /// <param name="function">The function tool to use.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceResponseFormat(FunctionTool function);

        /// <summary>
        /// Forces the response format to a specific function method.
        /// </summary>
        /// <param name="function">The function method to use.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceResponseFormat(MethodInfo function);

        /// <summary>
        /// Forces the response format to match a specific type.
        /// </summary>
        /// <typeparam name="T">The type to enforce.</typeparam>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceResponseFormat<T>();

        /// <summary>
        /// Forces the response format to be JSON.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceResponseAsJsonFormat();

        /// <summary>
        /// Forces the response format to be plain text.
        /// </summary>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun ForceResponseAsText();

        /// <summary>
        /// Adds text content to a specific role in the run.
        /// </summary>
        /// <param name="role">The role to associate with the text.</param>
        /// <param name="text">The text content to add.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun AddText(ChatRole role, string text);

        /// <summary>
        /// Creates a builder to add content for a specific role.
        /// </summary>
        /// <param name="role">The role to associate with the content (default is user).</param>
        /// <returns>A builder for creating content in the run.</returns>
        ChatMessageContentBuilder<IOpenAiRun> AddContent(ChatRole role = ChatRole.User);

        /// <summary>
        /// Creates a builder to add user-specific content.
        /// </summary>
        /// <returns>A builder for creating user-specific content in the run.</returns>
        ChatMessageContentBuilder<IOpenAiRun> AddUserContent();

        /// <summary>
        /// Creates a builder to add assistant-specific content.
        /// </summary>
        /// <returns>A builder for creating assistant-specific content in the run.</returns>
        ChatMessageContentBuilder<IOpenAiRun> AddAssistantContent();

        /// <summary>
        /// Adds metadata to a specific role in the run.
        /// </summary>
        /// <param name="role">The role to associate with the metadata.</param>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The current run instance for chaining.</returns>
        IOpenAiRun AddMetadata(ChatRole role, string key, string value);

        /// <summary>
        /// Starts the run asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the run result.</returns>
        ValueTask<RunResult> StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Starts the run as a stream asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>An asynchronous stream of run results.</returns>
        IAsyncEnumerable<RunResult> StartAsStreamAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Cancels a run asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the run to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the canceled run result.</returns>
        ValueTask<RunResult> CancelAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists runs asynchronously with optional filters.
        /// </summary>
        /// <param name="take">The number of runs to retrieve.</param>
        /// <param name="elementId">Optional: The ID of a reference run for pagination.</param>
        /// <param name="getAfterTheElementId">Specifies whether to retrieve runs after the reference run (true) or before (false).</param>
        /// <param name="order">The order of the runs (ascending or descending).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing an array of run results.</returns>
        ValueTask<ResponseAsArray<RunResult>> ListAsync(int take = 20, string? elementId = null, bool getAfterTheElementId = true, AssistantOrder order = AssistantOrder.Descending, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a run asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the run to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the run result.</returns>
        ValueTask<RunResult> RetrieveAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a run asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the run to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated run result.</returns>
        ValueTask<RunResult> UpdateAsync(string id, CancellationToken cancellationToken = default);
    }
}
