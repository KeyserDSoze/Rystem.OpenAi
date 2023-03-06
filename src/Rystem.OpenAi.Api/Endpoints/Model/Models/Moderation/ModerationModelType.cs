namespace Rystem.OpenAi.Models
{
    /// <summary>
    /// The Moderation models are designed to check whether content complies with <see href="https://platform.openai.com/docs/usage-policies/">OpenAI's usage policies</see>. The models provide classification capabilities that look for content in the following categories: hate, hate/threatening, self-harm, sexual, sexual/minors, violence, and violence/graphic. You can find out more in our <see href="https://platform.openai.com/docs/guides/moderation/overview">moderation guide</see>.
    /// </summary>
    public enum ModerationModelType
    {
        /// <summary>
        /// Almost as capable as the latest model, but slightly older.
        /// </summary>
        TextModerationStable = 0,
        /// <summary>
        /// Most capable moderation model. Accuracy will be slighlty higher than the stable model
        /// </summary>
        TextModerationLatest = 100,
    }
}
