using System;

namespace Rystem.OpenAi
{
    public interface IOpenAiLogger
    {
        IOpenAiLogger CreateId();
        IOpenAiLogger ConfigureId(string id);
        IOpenAiLogger ConfigureFactory(string? factoryName);
        IOpenAiLogger ConfigureTypes(OpenAiType[] types);
        IOpenAiLogger ConfigureUrl(string endpoint);
        IOpenAiLogger ConfigureMethod(string method);
        IOpenAiLogger WithStreaming();
        IOpenAiLogger WithoutStreaming();
        IOpenAiLogger AddContent(object? content);
        IOpenAiLogger AddResponse(object? response);
        IOpenAiLogger AddException(Exception exception);
        IOpenAiLogger AddError(string error);
        IOpenAiLogger Count();
        IOpenAiLogger StartTimer();
        void LogInformation();
        void LogError();
    }
}
