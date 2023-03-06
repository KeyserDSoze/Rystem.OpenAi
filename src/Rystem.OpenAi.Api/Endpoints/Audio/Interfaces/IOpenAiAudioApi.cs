using System.IO;

namespace Rystem.OpenAi.Audio
{

    public interface IOpenAiAudioApi
    {
        /// <summary>
        /// Transcribes audio into the input language or translates audio into into English.
        /// </summary>
        /// <param name="file">The audio file to translate, in one of these formats: mp3, mp4, mpeg, mpga, m4a, wav, or webm.</param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        AudioRequestBuilder Request(Stream file, string fileName = "default");
    }
}
