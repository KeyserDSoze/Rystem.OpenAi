using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Audio;

namespace Rystem.OpenAi.RealTime
{
    public interface IOpenAiRealTime : IOpenAiBase<IOpenAiRealTime, RealTimeModelName>
    {
        /// <summary>
        /// Creates a new real-time session asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the real-time session response.</returns>
        ValueTask<RealTimeSessionResponse> CreateSessionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the model to be used for the real-time session.
        /// </summary>
        /// <param name="model">The model name.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithModel(string model);

        /// <summary>
        /// Sets the temperature for the real-time session.
        /// </summary>
        /// <param name="value">The temperature value. Must be between 0 and 2.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the temperature value is out of range.</exception>
        IOpenAiRealTime WithTemperature(double value);

        /// <summary>
        /// Configures the session to use only text modality.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithOnlyText();

        /// <summary>
        /// Configures the session to use both text and voice modalities.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithTextAndVoice();

        /// <summary>
        /// Sets the instructions for the real-time session.
        /// </summary>
        /// <param name="instructions">The instructions text.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithInstructions(string instructions);

        /// <summary>
        /// Sets the voice to be used for the real-time session.
        /// </summary>
        /// <param name="audioVoice">The audio voice.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithVoice(AudioVoice audioVoice);

        /// <summary>
        /// Sets the input audio format for the real-time session.
        /// </summary>
        /// <param name="format">The input audio format.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithInputAudioFormat(RealTimeAudioFormat format);

        /// <summary>
        /// Sets the output audio format for the real-time session.
        /// </summary>
        /// <param name="format">The output audio format.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithOutputAudioFormat(RealTimeAudioFormat format);

        /// <summary>
        /// Configures the input audio transcription for the real-time session.
        /// </summary>
        /// <returns>An instance of <see cref="IOpenAiRealTimeInputAudioTranscription{IOpenAiRealTime}"/> to configure the input audio transcription.</returns>
        IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime> WithInputAudioTranscription();

        /// <summary>
        /// Configures the turn detection for the real-time session.
        /// </summary>
        /// <returns>An instance of <see cref="IOpenAiRealTimeTurnDetection{IOpenAiRealTime}"/> to configure the turn detection.</returns>
        IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithTurnDetection();

        /// <summary>
        /// Sets the maximum number of response output tokens for the real-time session.
        /// </summary>
        /// <param name="tokens">The maximum number of tokens.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithMaxResponseOutputTokens(int tokens);

        /// <summary>
        /// Sets the maximum number of response output tokens to infinite for the real-time session.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime WithInfiniteMaxResponseOutputTokens();

        /// <summary>
        /// Clears all tools from the real-time session.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime ClearTools();

        /// <summary>
        /// Adds a function tool to the real-time session.
        /// </summary>
        /// <param name="tool">The function tool.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime AddFunctionTool(FunctionTool tool);

        /// <summary>
        /// Adds a function tool to the real-time session.
        /// </summary>
        /// <param name="function">The method info of the function.</param>
        /// <param name="strict">Indicates whether the function is strict.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime AddFunctionTool(MethodInfo function, bool? strict = null);

        /// <summary>
        /// Adds a function tool to the real-time session.
        /// </summary>
        /// <typeparam name="T">The type of the function tool.</typeparam>
        /// <param name="name">The name of the function tool.</param>
        /// <param name="description">The description of the function tool.</param>
        /// <param name="strict">Indicates whether the function tool is strict.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime AddFunctionTool<T>(string name, string? description = null, bool? strict = null);

        /// <summary>
        /// Configures the session to avoid calling tools.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime AvoidCallingTools();

        /// <summary>
        /// Configures the session to force call tools.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime ForceCallTools();

        /// <summary>
        /// Configures the session to allow calling tools automatically.
        /// </summary>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime CanCallTools();

        /// <summary>
        /// Forces the session to call a specific function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The current instance of <see cref="IOpenAiRealTime"/>.</returns>
        IOpenAiRealTime ForceCallFunction(string name);
    }
}
