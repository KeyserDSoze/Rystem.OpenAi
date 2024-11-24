using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Audio
{
    public interface IOpenAiSpeech : IOpenAiBase<IOpenAiSpeech>
    {
        /// <summary>
        /// Executes the request and returns an MP3 audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> Mp3Async(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request and returns an Opus audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> OpusAsync(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request and returns an AAC audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> AacAsync(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request and returns a FLAC audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> FlacAsync(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request and returns a WAV audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> WavAsync(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request and returns a PCM audio stream.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Stream</returns>
        ValueTask<Stream> PcmAsync(string input, CancellationToken cancellationToken = default);

        /// <summary>
        /// The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns>IOpenAiSpeech</returns>
        /// <exception cref="ArgumentException"></exception>
        IOpenAiSpeech WithSpeed(double speed);

        /// <summary>
        /// The voice to use when generating the audio. Supported voices are alloy, echo, fable, onyx, nova, and shimmer.
        /// </summary>
        /// <param name="audioVoice"></param>
        /// <returns>IOpenAiSpeech</returns>
        IOpenAiSpeech WithVoice(AudioVoice audioVoice);
    }
}
