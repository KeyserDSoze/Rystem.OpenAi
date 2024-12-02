namespace Rystem.OpenAi.Image
{
    public static class FormatResultImageExtensions
    {
        private const string ResponseFormatUrl = "url";
        private const string ResponseFormatB64Json = "b64_json";
        public static string AsString(this FormatResultImage formatResultImage)
        {
            return formatResultImage switch
            {
                FormatResultImage.B64Json => ResponseFormatB64Json,
                _ => ResponseFormatUrl,
            };
        }
    }
}
