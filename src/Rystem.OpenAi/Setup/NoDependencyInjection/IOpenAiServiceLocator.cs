using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;
using Rystem.OpenAi.Embedding;
using Rystem.OpenAi.Files;
using Rystem.OpenAi.FineTune;
using Rystem.OpenAi.Image;
using Rystem.OpenAi.Models;
using Rystem.OpenAi.Moderation;

namespace Rystem.OpenAi
{
    public interface IOpenAiServiceLocator
    {
        /// <summary>
        /// Creates an instance of IOpenAi with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAi.</returns>
        IOpenAi Create(string? integrationName = default);

        /// <summary>
        /// Creates an instance of IOpenAiAudio with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiAudio.</returns>
        IOpenAiAudio CreateAudio(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiSpeech with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiSpeech.</returns>
        IOpenAiSpeech CreateSpeech(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiChat with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiChat.</returns>
        IOpenAiChat CreateChat(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiEmbedding with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiEmbedding.</returns>
        IOpenAiEmbedding CreateEmbedding(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiFile with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiFile.</returns>
        IOpenAiFile CreateFile(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiFineTune with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiFineTune.</returns>
        IOpenAiFineTune CreateFineTune(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiImage with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiImage.</returns>
        IOpenAiImage CreateImage(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiModel with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiModel.</returns>
        IOpenAiModel CreateModel(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiModeration with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiModeration.</returns>
        IOpenAiModeration CreateModeration(string? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiManagement with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiManagement.</returns>
        IOpenAiManagement CreateManagement(string? integrationName = null);

        /// <summary>
        /// Retrieves the utility services for OpenAI.
        /// </summary>
        /// <returns>An instance of IOpenAiUtility.</returns>
        IOpenAiUtility Utility();
    }
}
