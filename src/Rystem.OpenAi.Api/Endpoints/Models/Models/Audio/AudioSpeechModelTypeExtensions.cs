namespace Rystem.OpenAi
{
    public static class AudioSpeechModelTypeExtensions
    {
        private static readonly string s_tts = "tts-1";
        private static readonly string s_ttsHd = "tts-1-hd";
        public static string ToModel(this AudioSpeechModelType type)
        {
            switch (type)
            {
                default:
                case AudioSpeechModelType.Tts:
                    return s_tts;
                case AudioSpeechModelType.TtsHd:
                    return s_ttsHd;
            }
        }
    }
}
