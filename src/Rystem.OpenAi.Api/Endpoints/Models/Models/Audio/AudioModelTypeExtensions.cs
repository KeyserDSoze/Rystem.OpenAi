namespace Rystem.OpenAi
{
    public static class AudioModelTypeExtensions
    {
        private static readonly string s_whisper ="whisper-1";
        public static string ToModel(this AudioModelType type)
        {
            switch (type)
            {
                default:
                case AudioModelType.Whisper:
                    return s_whisper;
            }
        }
    }
}
