using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace Rystem.OpenAi
{
    public sealed class OpenAiConfiguration
    {
        private const string Forced = nameof(Forced);
        public Func<OpenAiType, string, bool, string> GetUri { get; }
        public Func<HttpClient, Task>? BeforeRequest { get; }
        public bool NeedClientEnrichment => BeforeRequest != null;
        public Task EnrichClientAsync(HttpClient client)
        {
            if (BeforeRequest != null)
                return BeforeRequest.Invoke(client);
            else
                return Task.CompletedTask;
        }
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

            var scopes = new[] { $"https://cognitiveservices.azure.com/.default" };
            if (settings.Azure.HasConfiguration)
            {
                if (settings.Azure.HasManagedIdentity)
                {
                    var credential = settings.Azure.ManagedIdentity.UseDefault ?
                        new DefaultAzureCredential() :
                        new DefaultAzureCredential(new DefaultAzureCredentialOptions
                        {
                            ManagedIdentityClientId = settings.Azure.ManagedIdentity.Id
                        });
                    BeforeRequest = async client =>
                    {
                        var accessToken = await credential.GetTokenAsync(new TokenRequestContext(scopes));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                    };
                }
                else if (settings.Azure.HasAppRegistration)
                {
                    var credential = ConfidentialClientApplicationBuilder.Create(settings.Azure.AppRegistration.ClientId)
                        .WithClientSecret(settings.Azure.AppRegistration.ClientSecret)
                        .WithAuthority(AadAuthorityAudience.AzureAdMyOrg, true)
                        .WithTenantId(settings.Azure.AppRegistration.TenantId)
                        .Build();
                    BeforeRequest = async client =>
                    {
                        var accessToken = await credential
                                            .AcquireTokenForClient(scopes)
                                            .ExecuteAsync();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);
                    };
                }

                var uris = new Dictionary<string, string>();
                settings.Version ??= "2022-12-01";
                settings.Azure.Deployments.Add("{0}", Forced);

                foreach (var deployment in settings.Azure.Deployments)
                {
                    uris.Add($"{deployment.Value}_{OpenAiType.Completion}", $"{string.Format(completionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Chat}", $"{string.Format(chatUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Edit}", $"{string.Format(editUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Embedding}", $"{string.Format(embeddingUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.File}", $"{string.Format(fileUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.FineTune}", $"{string.Format(fineTuneUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}/")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Model}", $"{string.Format(modelUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Moderation}", $"{string.Format(moderationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Image}", $"{string.Format(imageUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.AudioTranscription}", $"{string.Format(audioTranscriptionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
                    uris.Add($"{deployment.Value}_{OpenAiType.AudioTranslation}", $"{string.Format(audioTranslationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}")}?api-version={settings.Version}");
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
                        case OpenAiType.AudioTranscription:
                            return audioTranscriptionUri;
                        case OpenAiType.AudioTranslation:
                            return audioTranslationUri;
                        case OpenAiType.Completion:
                            return completionUri;
                        case OpenAiType.Edit:
                            return editUri;
                        case OpenAiType.Moderation:
                            return moderationUri;
                        case OpenAiType.Image:
                            return imageUri;
                        case OpenAiType.Embedding:
                            return embeddingUri;
                        case OpenAiType.Chat:
                            return chatUri;
                        case OpenAiType.File:
                            return fileUri;
                        case OpenAiType.FineTune:
                            return fineTuneUri;
                        default:
                        case OpenAiType.Model:
                            return modelUri;
                    }
                };
            }
        }
    }
}
