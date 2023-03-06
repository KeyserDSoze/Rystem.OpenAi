namespace Rystem.OpenAi.Models
{
    public static class AudioModelTypeExtensions
    {
        private static readonly Model s_whisper = new Model("whisper-1");
        public static Model ToModel(this AudioModelType type)
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
