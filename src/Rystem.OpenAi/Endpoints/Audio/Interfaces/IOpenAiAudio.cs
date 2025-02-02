﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Audio
{
    public interface IOpenAiAudio : IOpenAiBase<IOpenAiAudio, AudioModelName>
    {
        /// <summary>
        /// Add the file as array of bytes
        /// </summary>
        /// <param name="file">Array of bytes</param>
        /// <param name="fileName">Audio name</param>
        /// <returns></returns>
        IOpenAiAudio WithFile(byte[] file, string fileName = "default");
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
        /// <returns>VerboseSegmentAudioResult</returns>
        ValueTask<VerboseSegmentAudioResult> VerboseTranscriptAsSegmentsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Transcribes audio into a verbose representation in the input language
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseWordAudioResult</returns>
        ValueTask<VerboseWordAudioResult> VerboseTranscriptAsWordsAsync(CancellationToken cancellationToken = default);

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
        /// <returns>VerboseSegmentAudioResult</returns>
        ValueTask<VerboseSegmentAudioResult> VerboseTranslateAsSegmentsAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Translates audio into a verbose representation in English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseWordAudioResult</returns>
        ValueTask<VerboseWordAudioResult> VerboseTranslateAsWordsAsync(CancellationToken cancellationToken = default);

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
        /// Add minutes for your transcription
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        IOpenAiAudio WithTranscriptionMinutes(int minutes);
        /// <summary>
        /// /// Add minutes for your translation
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        IOpenAiAudio WithTranslationMinutes(int minutes);
    }
}
