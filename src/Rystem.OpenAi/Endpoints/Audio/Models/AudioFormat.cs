namespace Rystem.OpenAi.Audio
{
    public enum AudioFormat
    {
        Mp3,
        Opus,
        Aac,
        Flac,
        Wav,
        Pcm,
    }
    public static class AudioFormatExtensions
    {
        private const string Mp3 = "mp3";
        private const string Opus = "opus";
        private const string Aac = "aac";
        private const string Flac = "flac";
        private const string Wav = "wav";
        private const string Pcm = "pcm";

        public static string AsString(this AudioFormat audioFormat)
        {
            return audioFormat switch
            {
                AudioFormat.Mp3 => Mp3,
                AudioFormat.Opus => Opus,
                AudioFormat.Aac => Aac,
                AudioFormat.Flac => Flac,
                AudioFormat.Wav => Wav,
                AudioFormat.Pcm => Pcm,
                _ => Mp3,
            };
        }
    }
}
