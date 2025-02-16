using System;

namespace Rystem.OpenAi.RealTime
{
    internal sealed class InputAudioTranscriptionRequestBuilder : IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime>
    {
        private readonly IOpenAiRealTime _openAiRealTime;
        private readonly RealTimeSessionRequest _realTimeSessionRequest;

        public InputAudioTranscriptionRequestBuilder(IOpenAiRealTime openAiRealTime, RealTimeSessionRequest realTimeSessionRequest)
        {
            _openAiRealTime = openAiRealTime;
            _realTimeSessionRequest = realTimeSessionRequest;
        }
        public IOpenAiRealTime And()
            => _openAiRealTime;

        public IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime> WithLanguage(Language language)
        {
            _realTimeSessionRequest.InputAudioTranscription ??= new();
            _realTimeSessionRequest.InputAudioTranscription.Language = language.ToIso639_1();
            return this;
        }

        public IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime> WithModel(AudioModelName model)
        {
            _realTimeSessionRequest.InputAudioTranscription ??= new();
            _realTimeSessionRequest.InputAudioTranscription.Model = model;
            return this;
        }

        public IOpenAiRealTimeInputAudioTranscription<IOpenAiRealTime> WithPrompt(string prompt)
        {
            _realTimeSessionRequest.InputAudioTranscription ??= new();
            _realTimeSessionRequest.InputAudioTranscription.Prompt = prompt;
            return this;
        }
    }
    internal sealed class TurnDetectionRequestBuilder : IOpenAiRealTimeTurnDetection<IOpenAiRealTime>
    {
        private readonly IOpenAiRealTime _openAiRealTime;
        private readonly RealTimeSessionRequest _realTimeSessionRequest;
        public TurnDetectionRequestBuilder(IOpenAiRealTime openAiRealTime, RealTimeSessionRequest realTimeSessionRequest)
        {
            _openAiRealTime = openAiRealTime;
            _realTimeSessionRequest = realTimeSessionRequest;
        }
        public IOpenAiRealTime And()
            => _openAiRealTime;

        public IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithCreateResponse(bool createResponse)
        {
            _realTimeSessionRequest.TurnDetection ??= new();
            _realTimeSessionRequest.TurnDetection.CreateResponse = createResponse;
            return this;
        }

        public IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithPrefixPaddingMs(int ms)
        {
            _realTimeSessionRequest.TurnDetection ??= new();
            _realTimeSessionRequest.TurnDetection.PrefixPaddingMs = ms;
            return this;
        }

        public IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithSilenceDurationMs(int ms)
        {
            _realTimeSessionRequest.TurnDetection ??= new();
            _realTimeSessionRequest.TurnDetection.SilenceDurationMs = ms;
            return this;
        }

        public IOpenAiRealTimeTurnDetection<IOpenAiRealTime> WithThreshold(double threshold)
        {
            if (threshold < 0 || threshold > 1)
                throw new ArgumentException("Threshold must be between 0 and 1");
            _realTimeSessionRequest.TurnDetection ??= new();
            _realTimeSessionRequest.TurnDetection.Threshold = threshold;
            return this;
        }
    }
}
