namespace Rystem.OpenAi.RealTime
{
    public interface IOpenAiRealTimeTurnDetection<T>
    {
        /// <summary>
        /// Activation threshold for VAD (0.0 to 1.0), this defaults to 0.5. A higher threshold will require louder audio to activate the model, and thus might perform better in noisy environments.
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        IOpenAiRealTimeTurnDetection<T> WithThreshold(double threshold);
        /// <summary>
        /// Amount of audio to include before the VAD detected speech (in milliseconds). Defaults to 300ms.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        IOpenAiRealTimeTurnDetection<T> WithPrefixPaddingMs(int ms);
        /// <summary>
        /// Duration of silence to detect speech stop (in milliseconds). Defaults to 500ms. With shorter values the model will respond more quickly, but may jump in on short pauses from the user.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        IOpenAiRealTimeTurnDetection<T> WithSilenceDurationMs(int ms);
        /// <summary>
        /// Whether or not to automatically generate a response when VAD is enabled. true by default.
        /// </summary>
        /// <param name="createResponse"></param>
        /// <returns></returns>
        IOpenAiRealTimeTurnDetection<T> WithCreateResponse(bool createResponse);
        T And();
    }
}
