using System;
using Rystem.OpenAi.Assistant;
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
        IOpenAi Create(AnyOf<string?, Enum>? integrationName = default);

        /// <summary>
        /// Creates an instance of IOpenAiAudio with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiAudio.</returns>
        IOpenAiAudio CreateAudio(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiSpeech with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiSpeech.</returns>
        IOpenAiSpeech CreateSpeech(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiChat with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiChat.</returns>
        IOpenAiChat CreateChat(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiEmbedding with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiEmbedding.</returns>
        IOpenAiEmbedding CreateEmbedding(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiFile with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiFile.</returns>
        IOpenAiFile CreateFile(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiFineTune with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiFineTune.</returns>
        IOpenAiFineTune CreateFineTune(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiImage with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiImage.</returns>
        IOpenAiImage CreateImage(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiModel with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiModel.</returns>
        IOpenAiModel CreateModel(AnyOf<string?, Enum>? integrationName = null);

        /// <summary>
        /// Creates an instance of IOpenAiModeration with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiModeration.</returns>
        IOpenAiModeration CreateModeration(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Creates an instance of IOpenAiManagement with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiManagement.</returns>
        IOpenAiManagement CreateManagement(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Creates an instance of IOpenAiAssistant with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiAssistant.</returns>
        IOpenAiAssistant CreateAssistant(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Creates an instance of IOpenAiThread with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiThread.</returns>
        IOpenAiThread CreateThread(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Creates an instance of IOpenAiRun with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiRun.</returns>
        IOpenAiRun CreateRun(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Creates an instance of IOpenAiVectorStore with an optional integration name.
        /// </summary>
        /// <param name="integrationName">Optional name for the integration.</param>
        /// <returns>An instance of IOpenAiVectorStore.</returns>
        IOpenAiVectorStore CreateVectorStore(AnyOf<string?, Enum>? integrationName = null);
        /// <summary>
        /// Retrieves the utility services for OpenAI.
        /// </summary>
        /// <returns>An instance of IOpenAiUtility.</returns>
        IOpenAiUtility Utility();
    }
}
