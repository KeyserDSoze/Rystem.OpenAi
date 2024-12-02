using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiSpeech : OpenAiBuilder<IOpenAiSpeech, AudioSpeechRequest, SpeechModelName>, IOpenAiSpeech
    {
        public OpenAiSpeech(IFactory<DefaultServices> factory)
            : base(factory)
        {
            Request.Speed = 1;
            Request.Voice = AudioVoice.Alloy.ToString().ToLower();
            Request.Model = SpeechModelName.TtsHd;
        }
        private async ValueTask<Stream> ExecuteAsync(string input, string responseFormat, CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = responseFormat;
            Request.Input = input;
            Usages.Add(new OpenAiCost { Units = Request.Input?.Length ?? 0, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens });
            var response = await DefaultServices.HttpClient.PostAsync(DefaultServices.Configuration.GetUri(OpenAiType.AudioSpeech, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
            return response;
        }
        public ValueTask<Stream> Mp3Async(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "mp3", cancellationToken);
        public ValueTask<Stream> OpusAsync(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "opus", cancellationToken);
        public ValueTask<Stream> AacAsync(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "aac", cancellationToken);
        public ValueTask<Stream> FlacAsync(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "flac", cancellationToken);
        public ValueTask<Stream> WavAsync(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "wav", cancellationToken);
        public ValueTask<Stream> PcmAsync(string input, CancellationToken cancellationToken = default)
            => ExecuteAsync(input, "pcm", cancellationToken);
        public IOpenAiSpeech WithSpeed(double speed)
        {
            if (speed < 0.25d || speed > 4d)
                throw new ArgumentException($"Speed value is {speed}. But you may select a value from 0.25 to 4.0");
            Request.Speed = speed;
            return this;
        }
        public IOpenAiSpeech WithVoice(AudioVoice audioVoice)
        {
            Request.Voice = audioVoice.ToString().ToLower();
            return this;
        }
    }
}
