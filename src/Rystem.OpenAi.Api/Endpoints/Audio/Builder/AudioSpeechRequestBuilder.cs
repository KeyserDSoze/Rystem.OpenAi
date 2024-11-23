using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioSpeechRequestBuilder : RequestBuilder<AudioSpeechRequest>
    {
        internal AudioSpeechRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            string input, IOpenAiUtility utility) :
            base(client, configuration, () =>
            {
                var request = new AudioSpeechRequest()
                {
                    Model = AudioSpeechModelType.Tts.ToModel(),
                    Speed = 1,
                    Input = input,
                    Voice = AudioVoice.Alloy.ToString().ToLower(),
                };
                return request;
            }, utility)
        {
            _familyType = ModelFamilyType.Tts;
        }

        private async ValueTask<Stream> ExecuteAsync(string responseFormat, CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = responseFormat;
            var response = await Client.PostAsync(Configuration.GetUri(OpenAiType.AudioSpeech, Request.Model!, _forced, string.Empty), Request, Configuration, cancellationToken);
            return response;
        }
        public ValueTask<Stream> Mp3Async(CancellationToken cancellationToken = default)
            => ExecuteAsync("mp3", cancellationToken);
        public ValueTask<Stream> OpusAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync("opus", cancellationToken);
        public ValueTask<Stream> AacAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync("aac", cancellationToken);
        public ValueTask<Stream> FlacAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync("flac", cancellationToken);
        public ValueTask<Stream> WavAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync("wav", cancellationToken);
        public ValueTask<Stream> PcmAsync(CancellationToken cancellationToken = default)
            => ExecuteAsync("pcm", cancellationToken);
        /// <summary>
        /// The speed of the generated audio. Select a value from 0.25 to 4.0. 1.0 is the default.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public AudioSpeechRequestBuilder WithSpeed(double speed)
        {
            if (speed < 0.25d || speed > 4d)
                throw new ArgumentException($"Speed value is {speed}. But you may select a value from 0.25 to 4.0");
            Request.Speed = speed;
            return this;
        }
        /// <summary>
        /// The voice to use when generating the audio. Supported voices are alloy, echo, fable, onyx, nova, and shimmer.
        /// </summary>
        /// <param name="audioVoice"></param>
        /// <returns></returns>
        public AudioSpeechRequestBuilder WithVoice(AudioVoice audioVoice)
        {
            Request.Voice = audioVoice.ToString().ToLower();
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCost()
        {
            var cost = Utility.Cost;
            return cost.Configure(settings =>
            {
                settings
                    .WithType(OpenAiType.AudioSpeech);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                Units = Request.Input?.Length ?? 0
            });
        }
    }
}
