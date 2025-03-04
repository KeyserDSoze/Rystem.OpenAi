namespace Rystem.OpenAi.RealTime
{
    public enum RealTimeAudioFormat
    {
        Pcm16,
        G711Ulaw,
        G711Alaw
    }
    public static class RealTimeAudioFormatExtensions
    {
        private const string Pcm16 = "pcm16";
        private const string G711Ulaw = "g711_ulaw";
        private const string G711Alaw = "g711_alaw";

        public static string AsString(this RealTimeAudioFormat audioFormat)
        {
            return audioFormat switch
            {
                RealTimeAudioFormat.Pcm16 => Pcm16,
                RealTimeAudioFormat.G711Ulaw => G711Ulaw,
                RealTimeAudioFormat.G711Alaw => G711Alaw,
                _ => Pcm16,
            };
        }
    }
}
