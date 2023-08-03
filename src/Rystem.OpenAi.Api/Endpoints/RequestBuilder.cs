using System;
using System.Net.Http;

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
        private protected RequestBuilder(HttpClient client,
            OpenAiConfiguration configuration,
            Func<T> requestCreator,
            IOpenAiUtility utility)
        {
            Client = client;
            Configuration = configuration;
            Utility = utility;
            Request = requestCreator.Invoke();
        }
        private protected decimal CalculateCost(OpenAiType type, Usage? usage)
        {
            var cost = Utility.Cost;
            var result =
                Utility.Executor.Execute($"Api: {type} calculation cost", () =>
                {
                    var response = cost.Configure(settings =>
                    {
                        settings
                            .WithFamily(_familyType)
                            .WithType(type);
                    }, Configuration.Name).Invoke(new OpenAiUsage
                    {
                        PromptTokens = usage?.PromptTokens ?? 0,
                        CompletionTokens = (usage as CompletionUsage)?.CompletionTokens ?? 0
                    });
                    return response;
                });
            return result;
        }
    }
}
