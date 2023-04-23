using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace Rystem.OpenAi
{
    internal delegate string OpenaiUriMaker(OpenAiType type, string modelId, bool forced, string appendBeforeQueryString);
    public sealed class OpenAiConfiguration
    {
        private const string Forced = nameof(Forced);
        internal OpenaiUriMaker GetUri { get; }
        public Func<HttpClient, Task>? BeforeRequest { get; }
        public bool NeedClientEnrichment => BeforeRequest != null;
        public bool WithAzure { get; }
        public string Name { get; }
        public Task EnrichClientAsync(HttpClient client)
        {
            if (BeforeRequest != null)
                return BeforeRequest.Invoke(client);
            else
                return Task.CompletedTask;
        }
        internal OpenAiConfiguration(OpenAiSettings settings, string? name)
        {
            Name = name ?? string.Empty;
            var uri = "{0}/{1}{2}{3}";
            var completionUri = string.Format(uri, "{0}", "completions", "{1}", "{2}");
            var chatUri = string.Format(uri, "{0}", "chat/completions", "{1}", "{2}");
            var editUri = string.Format(uri, "{0}", "edits", "{1}", "{2}");
            var embeddingUri = string.Format(uri, "{0}", "embeddings", "{1}", "{2}");
            var fileUri = string.Format(uri, "{0}", "files", "{1}", "{2}");
            var fineTuneUri = string.Format(uri, "{0}", "fine-tunes", "{1}", "{2}");
            var modelUri = string.Format(uri, "{0}", "models", "{1}", "{2}");
            var moderationUri = string.Format(uri, "{0}", "moderations", "{1}", "{2}");
            var imageUri = string.Format(uri, "{0}", "images", "{1}", "{2}");
            var audioTranscriptionUri = string.Format(uri, "{0}", "audio/transcriptions", "{1}", "{2}");
            var audioTranslationUri = string.Format(uri, "{0}", "audio/translations", "{1}", "{2}");
            var billingUri = string.Format(uri, "{0}", "dashboard/billing/usage", "{1}", "{2}");
            var deploymentUri = string.Format(uri, "{0}", "deployments", "{1}", "{2}");

            var scopes = new[] { $"https://cognitiveservices.azure.com/.default" };
            if (settings.Azure.HasConfiguration)
            {
                WithAzure = true;
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

                modelUri = string.Format(modelUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai", "{0}", $"?api-version={GetVersion(settings, OpenAiType.Model)}");
                fineTuneUri = string.Format(fineTuneUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai", "{0}", $"?api-version={GetVersion(settings, OpenAiType.FineTune)}");
                fileUri = string.Format(fileUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai", "{0}", $"?api-version={GetVersion(settings, OpenAiType.File)}");
                billingUri = string.Format(billingUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai", "{0}", $"?api-version={GetVersion(settings, OpenAiType.Billing)}");
                deploymentUri = string.Format(deploymentUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai", "{0}", $"?api-version={GetVersion(settings, OpenAiType.Billing)}");

                foreach (var deployment in settings.Azure.Deployments)
                {
                    uris.Add($"{deployment.Value}_{OpenAiType.Completion}", $"{string.Format(completionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Completion)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Chat}", $"{string.Format(chatUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Chat)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Edit}", $"{string.Format(editUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Edit)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Embedding}", $"{string.Format(embeddingUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Embedding)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Moderation}", $"{string.Format(moderationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Moderation)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.Image}", $"{string.Format(imageUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.Image)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.AudioTranscription}", $"{string.Format(audioTranscriptionUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.AudioTranscription)}")}");
                    uris.Add($"{deployment.Value}_{OpenAiType.AudioTranslation}", $"{string.Format(audioTranslationUri, $"https://{settings.Azure.ResourceName}.OpenAi.Azure.com/openai/deployments/{deployment.Key}", "{1}", $"?api-version={GetVersion(settings, OpenAiType.AudioTranslation)}")}");
                }

                GetUri = (type, modelId, forceModel, appendBeforeQueryString) =>
                {
                    switch (type)
                    {
                        case OpenAiType.File:
                            return string.Format(fileUri, appendBeforeQueryString);
                        case OpenAiType.FineTune:
                            return string.Format(fineTuneUri, appendBeforeQueryString);
                        case OpenAiType.Billing:
                            return string.Format(billingUri, appendBeforeQueryString);
                        case OpenAiType.Model:
                            return string.Format(modelUri, appendBeforeQueryString);
                        case OpenAiType.Deployment:
                            return string.Format(deploymentUri, appendBeforeQueryString);
                    }
                    if (forceModel)
                        return string.Format(uris[$"{Forced}_{type}"], modelId, appendBeforeQueryString);
                    var key = $"{modelId}_{type}";
                    if (uris.ContainsKey(key))
                        return string.Format(uris[key], string.Empty, appendBeforeQueryString);
                    else
                        throw new ArgumentException($"Model {modelId} of {type} is not installed during startup phase. In services.AddOpenAi configure the Azure environment with the correct DeploymentModel.");
                };
            }
            else
            {
                settings.Version ??= "v1";
                completionUri = string.Format(completionUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Completion)}", "{0}", string.Empty);
                chatUri = string.Format(chatUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Chat)}", "{0}", string.Empty);
                editUri = string.Format(editUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Edit)}", "{0}", string.Empty);
                embeddingUri = string.Format(embeddingUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Embedding)}", "{0}", string.Empty);
                fileUri = string.Format(fileUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.File)}", "{0}", string.Empty);
                fineTuneUri = string.Format(fineTuneUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.FineTune)}", "{0}", string.Empty);
                modelUri = string.Format(modelUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Model)}", "{0}", string.Empty);
                moderationUri = string.Format(moderationUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Moderation)}", "{0}", string.Empty);
                imageUri = string.Format(imageUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.Image)}", "{0}", string.Empty);
                audioTranscriptionUri = string.Format(audioTranscriptionUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.AudioTranscription)}", "{0}", string.Empty);
                audioTranslationUri = string.Format(audioTranslationUri, $"https://api.openai.com/{GetVersion(settings, OpenAiType.AudioTranslation)}", "{0}", string.Empty);
                billingUri = string.Format(billingUri, $"https://api.openai.com", "{0}", string.Empty);

                GetUri = (type, modelId, forceModel, appendBeforeQueryString) =>
                {
                    switch (type)
                    {
                        case OpenAiType.AudioTranscription:
                            return string.Format(audioTranscriptionUri, appendBeforeQueryString);
                        case OpenAiType.AudioTranslation:
                            return string.Format(audioTranslationUri, appendBeforeQueryString);
                        case OpenAiType.Completion:
                            return string.Format(completionUri, appendBeforeQueryString);
                        case OpenAiType.Edit:
                            return string.Format(editUri, appendBeforeQueryString);
                        case OpenAiType.Moderation:
                            return string.Format(moderationUri, appendBeforeQueryString);
                        case OpenAiType.Image:
                            return string.Format(imageUri, appendBeforeQueryString);
                        case OpenAiType.Embedding:
                            return string.Format(embeddingUri, appendBeforeQueryString);
                        case OpenAiType.Chat:
                            return string.Format(chatUri, appendBeforeQueryString);
                        case OpenAiType.File:
                            return string.Format(fileUri, appendBeforeQueryString);
                        case OpenAiType.FineTune:
                            return string.Format(fineTuneUri, appendBeforeQueryString);
                        case OpenAiType.Billing:
                            return string.Format(billingUri, appendBeforeQueryString);
                        default:
                        case OpenAiType.Model:
                            return string.Format(modelUri, appendBeforeQueryString);
                    }
                };
            }
        }
        private string? GetVersion(OpenAiSettings settings, OpenAiType type)
        {
            if (settings.Versions.ContainsKey(type))
                return settings.Versions[type];
            else
                return settings.Version;
        }
    }
}
