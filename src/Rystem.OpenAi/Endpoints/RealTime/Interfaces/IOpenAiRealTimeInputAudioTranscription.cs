namespace Rystem.OpenAi.RealTime
{
    public interface IOpenAiRealTimeInputAudioTranscription<T>
    {
        /// <summary>
        /// The model to use for transcription, whisper-1 is the only currently supported model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IOpenAiRealTimeInputAudioTranscription<T> WithModel(AudioModelName model);
        /// <summary>
        /// The language of the input audio. Supplying the input language in ISO-639-1 (e.g. en) format will improve accuracy and latency.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        IOpenAiRealTimeInputAudioTranscription<T> WithLanguage(Language language);
        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        IOpenAiRealTimeInputAudioTranscription<T> WithPrompt(string prompt);
        T And();
    }
}
