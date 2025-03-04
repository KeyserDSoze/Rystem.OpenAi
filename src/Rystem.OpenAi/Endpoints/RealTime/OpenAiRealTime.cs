using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Audio;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.RealTime
{
    internal sealed class OpenAiRealTime : OpenAiBuilder<IOpenAiRealTime, RealTimeSessionRequest, RealTimeModelName>, IOpenAiRealTime
    {
        private static readonly string[] s_allModalities = ["audio", "text"];
        private static readonly string[] s_onlyText = ["text"];
        public OpenAiRealTime(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory)
            : base(factory, configurationFactory, loggerFactory, OpenAiType.RealTime)
        {
            Request.Model = RealTimeModelName.Gpt_4o_realtime_preview_2024_12_17;
            Request.Modalities = s_allModalities;
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.RealTime != null)
            {
                configuration.Settings.DefaultRequestConfiguration.RealTime.Invoke(this);
            }
        }
        private const string SessionPath = "/sessions";
        public ValueTask<RealTimeSessionResponse> CreateSessionAsync(CancellationToken cancellationToken = default)
        {
            return DefaultServices.HttpClientWrapper
                    .PostAsync<RealTimeSessionResponse>(
                        DefaultServices.Configuration.GetUri(OpenAiType.RealTime, _version, Request.Model!, SessionPath, null),
                        Request,
                        null,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }

        private const string HttpProtocolStarter = "https://";
        private const string WebSocketProtocolStarter = "wss://";

        public RealTimeClient GetClientWithEphemeralKey(string ephemeralKey)
        {
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.RealTime, _version, Request.Model!, string.Empty, null);
            uri = uri.Replace(HttpProtocolStarter, WebSocketProtocolStarter);
            return new RealTimeClient(uri, null, ephemeralKey);
        }
        public async Task<RealTimeClient> GetAuthenticatedClientAsync()
        {
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.RealTime, _version, Request.Model!, string.Empty, null);
            uri = uri.Replace(HttpProtocolStarter, WebSocketProtocolStarter);
            var apiKey = DefaultServices.Configuration.GetToken != null ? await DefaultServices.Configuration.GetToken() : null;
            return new RealTimeClient(uri, apiKey, null);
        }
        public IOpenAiRealTime WithTemperature(double value)
        {
            if (value < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (value > 2)
                throw new ArgumentException("Temperature with a value greater than 2");
            Request.Temperature = value;
            return this;
        }
        public IOpenAiRealTime WithOnlyText()
        {
            Request.Modalities = s_onlyText;
            return this;
        }
        public IOpenAiRealTime WithTextAndVoice()
        {
            Request.Modalities = s_allModalities;
            return this;
        }
        public IOpenAiRealTime WithInstructions(string instructions)
        {
            Request.Instructions = instructions;
            return this;
        }
        public IOpenAiRealTime WithVoice(AudioVoice audioVoice)
        {
            Request.Voice = audioVoice.AsString();
            return this;
        }
        public IOpenAiRealTime WithInputAudioFormat(RealTimeAudioFormat format)
        {
            Request.InputAudioFormat = format.AsString();
            return this;
        }
        public IOpenAiRealTime WithOutputAudioFormat(RealTimeAudioFormat format)
        {
            Request.OutputAudioFormat = format.AsString();
            return this;
        }
        public IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime> WithInputAudioTranscription()
            => new InputAudioTranscriptionRequestBuilder(this, Request);
        public IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithTurnDetection()
            => new TurnDetectionRequestBuilder(this, Request);
        public IOpenAiRealTime WithMaxResponseOutputTokens(int tokens)
        {
            Request.MaxResponseOutputTokens = tokens;
            return this;
        }
        private const string InfiniteMaxResponseOutputTokens = "inf";
        public IOpenAiRealTime WithInfiniteMaxResponseOutputTokens()
        {
            Request.MaxResponseOutputTokens = InfiniteMaxResponseOutputTokens;
            return this;
        }
        public IOpenAiRealTime ClearTools()
        {
            Request.Tools?.Clear();
            return this;
        }
        public IOpenAiRealTime AddFunctionTool(FunctionTool tool)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = tool });
            return this;
        }
        public IOpenAiRealTime AddFunctionTool(MethodInfo function, bool? strict = null)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = function.ToFunctionTool(null, strict) });
            return this;
        }
        public IOpenAiRealTime AddFunctionTool<T>(string name, string? description = null, bool? strict = null)
        {
            if (Request.ToolChoice?.ToString() == ChatConstants.ToolChoice.None)
                Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            Request.Tools ??= [];
            Request.Tools.Add(new ChatFunctionTool { Function = typeof(T).ToFunctionTool(name, description, strict) });
            return this;
        }
        public IOpenAiRealTime AvoidCallingTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.None;
            return this;
        }
        public IOpenAiRealTime ForceCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Required;
            return this;
        }
        public IOpenAiRealTime CanCallTools()
        {
            Request.ToolChoice = ChatConstants.ToolChoice.Auto;
            return this;
        }
        private const string FunctionType = "function";
        public IOpenAiRealTime ForceCallFunction(string name)
        {
            Request.ToolChoice = new ForcedFunctionTool
            {
                Type = FunctionType,
                Function = new ForcedFunctionToolName
                {
                    Name = name
                }
            };
            return this;
        }
    }
}
