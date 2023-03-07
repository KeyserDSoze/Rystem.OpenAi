using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Image;
using Rystem.OpenAi;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioRequestBuilder : RequestBuilder<AudioRequest>
    {
        internal AudioRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            Stream audio, string audioName) :
            base(client, configuration, () =>
            {
                var request = new AudioRequest()
                {
                    ModelId = AudioModelType.Whisper.ToModel().Id
                };
                var memoryStream = new MemoryStream();
                audio.CopyTo(memoryStream);
                request.Audio = memoryStream;
                request.AudioName = audioName ?? "default";
                return request;
            })
        {
        }
        internal const string ResponseFormatJson = "json";
        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        public async ValueTask<AudioResult> TranscriptAsync(CancellationToken cancellationToken = default)
        {
            _request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (_request.Audio != null)
            {
                using var imageData = new MemoryStream();
                await _request.Audio.CopyToAsync(imageData, cancellationToken);
                imageData.Position = 0;
                content.Add(new ByteArrayContent(imageData.ToArray()), "file", _request.AudioName);
            }
            if (_request.Prompt != null)
                content.Add(new StringContent(_request.Prompt), "prompt");
            if (_request.ResponseFormat != null)
                content.Add(new StringContent(_request.ResponseFormat), "response_format");
            if (_request.Language != null)
                content.Add(new StringContent(_request.Language), "language");
            if (_request.Temperature != null)
                content.Add(new StringContent(_request.Temperature.ToString()), "temperature");

            _request.Dispose();

            var response = await _client.PostAsync<AudioResult>(_configuration.GetUri(OpenAi.AudioTranscription, _request.ModelId!), content, cancellationToken);
            return response;
        }
        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        public async ValueTask<AudioResult> TranslateAsync(CancellationToken cancellationToken = default)
        {
            _request.ResponseFormat = ImageCreateRequestBuilder.ResponseFormatUrl;
            using var content = new MultipartFormDataContent();
            if (_request.Audio != null)
            {
                using var imageData = new MemoryStream();
                await _request.Audio.CopyToAsync(imageData, cancellationToken);
                imageData.Position = 0;
                content.Add(new ByteArrayContent(imageData.ToArray()), "file", _request.AudioName);
            }
            if (_request.Prompt != null)
                content.Add(new StringContent(_request.Prompt), "prompt");
            if (_request.ResponseFormat != null)
                content.Add(new StringContent(_request.ResponseFormat), "response_format");
            if (_request.Temperature != null)
                content.Add(new StringContent(_request.Temperature.ToString()), "temperature");

            _request.Dispose();

            var response = await _client.PostAsync<AudioResult>(_configuration.GetUri(OpenAi.AudioTranslation, _request.ModelId!), content, cancellationToken);
            return response;
        }
        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>AudioRequestBuilder</returns>
        public AudioRequestBuilder WithPrompt(string prompt)
        {
            _request.Prompt = prompt;
            return this;
        }
        /// <summary>
        /// The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use <see href="https://en.wikipedia.org/wiki/Log_probability">log probability</see> to automatically increase the temperature until certain thresholds are hit.
        /// </summary>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public AudioRequestBuilder WithTemperature(double temperature)
        {
            if (temperature < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (temperature > 1)
                throw new ArgumentException("Temperature with a value greater than 1");
            _request.Temperature = temperature;
            return this;
        }
        /// <summary>
        /// The language of the input audio. Supplying the input language in <see href="https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes">ISO-639-1</see> format will improve accuracy and latency.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public AudioRequestBuilder WithLanguage(Language language)
        {
            _request.Language = language.ToIso639_1();
            return this;
        }
    }
}
