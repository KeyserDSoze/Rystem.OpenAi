namespace Rystem.OpenAi.Audio
{
    public enum AudioVoice
    {
        Alloy,
        Ash,
        Coral,
        Ballad,
        Echo,
        Fable,
        Onyx,
        Nova,
        Sage,
        Shimmer,
        Verse
    }
    public static class AudioVoiceExtensions
    {
        private const string Alloy = "alloy";
        private const string Ash = "ash";
        private const string Coral = "coral";
        private const string Ballad = "ballad";
        private const string Echo = "echo";
        private const string Fable = "fable";
        private const string Onyx = "onyx";
        private const string Nova = "nova";
        private const string Sage = "sage";
        private const string Shimmer = "shimmer";
        private const string Verse = "verse";

        public static string AsString(this AudioVoice audioVoice)
        {
            return audioVoice switch
            {
                AudioVoice.Alloy => Alloy,
                AudioVoice.Ash => Ash,
                AudioVoice.Coral => Coral,
                AudioVoice.Ballad => Ballad,
                AudioVoice.Echo => Echo,
                AudioVoice.Fable => Fable,
                AudioVoice.Onyx => Onyx,
                AudioVoice.Nova => Nova,
                AudioVoice.Sage => Sage,
                AudioVoice.Shimmer => Shimmer,
                AudioVoice.Verse => Verse,
                _ => Alloy,
            };
        }
    }
}
