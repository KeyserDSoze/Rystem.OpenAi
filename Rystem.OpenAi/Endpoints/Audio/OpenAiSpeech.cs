using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiSpeech : OpenAiBuilder<IOpenAiSpeech, AudioSpeechRequest, ChatUsage>, IOpenAiSpeech
    {
        public OpenAiSpeech(IFactory<DefaultServices> factory)
            : base(factory)
        {
            //var request = new AudioRequest()
            //{
            //    Model = AudioModelType.Whisper.ToModel()
            //};
            //var memoryStream = new MemoryStream();
            //audio.CopyTo(memoryStream);
            //request.Audio = memoryStream;
            //request.AudioName = audioName ?? "default";
            //return request;
        }
        private async ValueTask<Stream> ExecuteAsync(string responseFormat, CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = responseFormat;
            var response = await DefaultServices.HttpClient.PostAsync(DefaultServices.Configuration.GetUri(OpenAiType.AudioSpeech, Request.Model!, Forced, string.Empty), Request, DefaultServices.Configuration, cancellationToken);
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
        public IOpenAiSpeech WithSpeed(double speed)
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
        public IOpenAiSpeech WithVoice(AudioVoice audioVoice)
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
            //todo: to refactor the audio input choose i don't like it
            decimal outputPrice = 0;
            foreach (var responses in Usages)
            {
                outputPrice += DefaultServices.Price.CalculatePrice(Request.Model!,
                    new OpenAiCost { Units = Request.Input?.Length ?? 0, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Tokens });
            }
            return outputPrice;
        }
    }
}
