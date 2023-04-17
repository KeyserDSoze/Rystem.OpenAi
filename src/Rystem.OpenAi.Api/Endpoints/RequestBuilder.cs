using System;
using System.Net.Http;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi
{
    public abstract class RequestBuilder<T>
        where T : class, IOpenAiRequest
    {
        private protected readonly HttpClient Client;
        private protected readonly OpenAiConfiguration Configuration;
        private protected readonly IOpenAiUtility Utility;
        private protected readonly T Request;
        private protected bool _forced;
        private protected ModelFamilyType _familyType;
        private protected readonly OpenAiUsage Usage = new OpenAiUsage();
        private protected RequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            Func<T> requestCreator, IOpenAiUtility utility)
        {
            Client = client;
            Configuration = configuration;
            Utility = utility;
            Request = requestCreator.Invoke();
        }
        private protected decimal CalculateCost(OpenAiType type, CompletionUsage? usage)
        {
            var cost = Utility.Cost;
            return cost.Configure(settings =>
            {
                settings
                    .WithFamily(_familyType)
                    .WithType(type);
            }).Invoke(new OpenAiUsage
            {
                PromptTokens = usage?.PromptTokens ?? 0,
                CompletionTokens = usage?.CompletionTokens ?? 0
            });
        }
    }
}
