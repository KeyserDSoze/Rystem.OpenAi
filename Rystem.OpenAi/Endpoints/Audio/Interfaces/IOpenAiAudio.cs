using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Audio
{
    public interface IOpenAiAudio
    {
        /// <summary>
        /// Add the file as stream
        /// </summary>
        /// <param name="file">Stream</param>
        /// <param name="fileName">Audio name</param>
        /// <returns></returns>
        Task<IOpenAiAudio> WithStreamAsync(Stream file, string fileName = "default");
        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        ValueTask<AudioResult> TranscriptAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Transcribes audio into a verbose representation in the input language
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseAudioResult</returns>
        ValueTask<VerboseAudioResult> VerboseTranscriptAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        ValueTask<AudioResult> TranslateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Translates audio into a verbose representation in English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseAudioResult</returns>
        ValueTask<VerboseAudioResult> VerboseTranslateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>IOpenAiAudio</returns>
        IOpenAiAudio WithPrompt(string prompt);

        /// <summary>
        /// The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use <see href="https://en.wikipedia.org/wiki/Log_probability">log probability</see> to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <param name="temperature"></param>
        /// <returns>IOpenAiAudio</returns>
        IOpenAiAudio WithTemperature(double temperature);

        /// <summary>
        /// The language of the input audio. Supplying the input language in <see href="https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes">ISO-639-1</see> format will improve accuracy and latency.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>IOpenAiAudio</returns>
        IOpenAiAudio WithLanguage(Language language);

        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        decimal CalculateCostForTranscription(int minutes = 0);

        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        decimal CalculateCostForTranslation(int minutes = 0);
    }
}
