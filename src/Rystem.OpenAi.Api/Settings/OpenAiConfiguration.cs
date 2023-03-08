using System;
using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public enum OpenAi
    {
        Completion,
        Chat,
        Edit,
        Embedding,
        File,
        FineTune,
        Model,
        Moderation,
        Image,
        AudioTranscription,
        AudioTranslation
    }
    public sealed class OpenAiConfiguration
    {
        private const string Forced = nameof(Forced);
        public Func<OpenAi, string, bool, string> GetUri { get; }
        internal OpenAiConfiguration(OpenAiSettings settings)
        {
            var uri = $"{{1}}/{{0}}";
            var completionUri = string.Format(uri, "completions", "{0}");
            var chatUri = string.Format(uri, "chat/completions", "{0}");
            var editUri = string.Format(uri, "edits", "{0}");
            var embeddingUri = string.Format(uri, "embeddings", "{0}");
            var fileUri = string.Format(uri, "files", "{0}");
            var fineTuneUri = string.Format(uri, "fine-tunes", "{0}");
            var modelUri = string.Format(uri, "models", "{0}");
            var moderationUri = string.Format(uri, "moderations", "{0}");
            var imageUri = string.Format(uri, "images", "{0}");
            var audioTranscriptionUri = string.Format(uri, "audio/transcriptions", "{0}");
            var audioTranslationUri = string.Format(uri, "audio/translations", "{0}");

            if (settings.Azure.HasConfiguration)
            {
                var uris = new Dictionary<string, string>();
                if (settings.Azure.Deployments.Count < 1)
                    throw new ArgumentNullException($"When you set an Azure resource name you have to add at least one {nameof(OpenAiSettings.Azure.Deployments)} in configuration setup. Go to Model deployments blade in your Open Ai Azure resource.");

                settings.Version ??= "2022-12-01";
                settings.Azure.Deployments.Add("{0}", Forced);
                foreach (var deployment in settings.Azure.Deployments)
                {
                    uris.Add($"{deployment.Value}_{OpenAi.Completion}", $"{string.Format(completionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Chat}", $"{string.Format(chatUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Edit}", $"{string.Format(editUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Embedding}", $"{string.Format(embeddingUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.File}", $"{string.Format(fileUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.FineTune}", $"{string.Format(fineTuneUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}/")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Model}", $"{string.Format(modelUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Moderation}", $"{string.Format(moderationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.Image}", $"{string.Format(imageUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.AudioTranscription}", $"{string.Format(audioTranscriptionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAi.AudioTranslation}", $"{string.Format(audioTranslationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                }

                GetUri = (type, modelId, forceModel) =>
                {
                    if (forceModel)
                        return string.Format(uris[$"{Forced}_{type}"], modelId);
                    var key = $"{modelId}_{type}";
                    if (uris.ContainsKey(key))
                        return uris[key];
                    else
                        throw new ArgumentException($"Model {modelId} of {type} is not installed during startup phase. In services.AddOpenAi configure the Azure environment with the correct DeploymentModel.");
                };
            }
            else
            {
                settings.Version ??= "v1";
                completionUri = string.Format(completionUri, $"https://api.openai.com/{settings.Version}");
                chatUri = string.Format(chatUri, $"https://api.openai.com/{settings.Version}");
                editUri = string.Format(editUri, $"https://api.openai.com/{settings.Version}");
                embeddingUri = string.Format(embeddingUri, $"https://api.openai.com/{settings.Version}");
                fileUri = string.Format(fileUri, $"https://api.openai.com/{settings.Version}");
                fineTuneUri = string.Format(fineTuneUri, $"https://api.openai.com/{settings.Version}");
                modelUri = string.Format(modelUri, $"https://api.openai.com/{settings.Version}");
                moderationUri = string.Format(moderationUri, $"https://api.openai.com/{settings.Version}");
                imageUri = string.Format(imageUri, $"https://api.openai.com/{settings.Version}");
                audioTranscriptionUri = string.Format(audioTranscriptionUri, $"https://api.openai.com/{settings.Version}");
                audioTranslationUri = string.Format(audioTranslationUri, $"https://api.openai.com/{settings.Version}");

                GetUri = (type, modelId, forceModel) =>
                {
                    switch (type)
                    {
                        case OpenAi.AudioTranscription:
                            return audioTranscriptionUri;
                        case OpenAi.AudioTranslation:
                            return audioTranslationUri;
                        case OpenAi.Completion:
                            return completionUri;
                        case OpenAi.Edit:
                            return editUri;
                        case OpenAi.Moderation:
                            return moderationUri;
                        case OpenAi.Image:
                            return imageUri;
                        case OpenAi.Embedding:
                            return embeddingUri;
                        case OpenAi.Chat:
                            return chatUri;
                        case OpenAi.File:
                            return fileUri;
                        case OpenAi.FineTune:
                            return fineTuneUri;
                        default:
                        case OpenAi.Model:
                            return modelUri;
                    }
                };
            }
        }
    }
}
