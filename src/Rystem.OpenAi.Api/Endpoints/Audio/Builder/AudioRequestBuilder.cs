using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Image;

namespace Rystem.OpenAi.Audio
{
    public sealed class AudioRequestBuilder : RequestBuilder<AudioRequest>
    {
        internal AudioRequestBuilder(HttpClient client, OpenAiConfiguration configuration,
            Stream audio, string audioName, IOpenAiUtility utility) :
            base(client, configuration, () =>
            {
                var request = new AudioRequest()
                {
                    Model = AudioModelType.Whisper.ToModel()
                };
                var memoryStream = new MemoryStream();
                audio.CopyTo(memoryStream);
                request.Audio = memoryStream;
                request.AudioName = audioName ?? "default";
                return request;
            }, utility)
        {
            _familyType = ModelFamilyType.Whisper;
        }
        internal const string ResponseFormatJson = "json";
        /// <summary>
        /// Transcribes audio into the input language.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        public async ValueTask<AudioResult> TranscriptAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatJson;
            using var content = new MultipartFormDataContent();
            if (Request.Audio != null)
            {
                var byteContent = new ByteArrayContent(Request.Audio.ToArray());
                content.Add(byteContent, "file", Request.AudioName ?? Guid.NewGuid().ToString());
            }
            if (Request.Model != null)
                content.Add(new StringContent(Request.Model.ToString()), "model");
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), "prompt");
            if (Request.ResponseFormat != null)
                content.Add(new StringContent(Request.ResponseFormat), "response_format");
            if (Request.Language != null)
                content.Add(new StringContent(Request.Language), "language");
            if (Request.Temperature != null)
                content.Add(new StringContent(Request.Temperature.ToString()!), "temperature");

            Request.Dispose();

            var response = await Client.PostAsync<AudioResult>(Configuration.GetUri(OpenAiType.AudioTranscription, Request.Model!, _forced, string.Empty), content, Configuration, cancellationToken);
            return response;
        }
        internal const string ResponseFormatVerboseJson = "verbose_json";
        /// <summary>
        /// Transcribes audio into a verbose representation in the input language
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseAudioResult</returns>
        public async ValueTask<VerboseAudioResult> VerboseTranscriptAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatVerboseJson;
            using var content = new MultipartFormDataContent();
            if (Request.Audio != null)
            {
                var byteContent = new ByteArrayContent(Request.Audio.ToArray());
                content.Add(byteContent, "file", Request.AudioName ?? Guid.NewGuid().ToString());
            }
            if (Request.Model != null)
                content.Add(new StringContent(Request.Model.ToString()), "model");
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), "prompt");
            if (Request.ResponseFormat != null)
                content.Add(new StringContent(Request.ResponseFormat), "response_format");
            if (Request.Language != null)
                content.Add(new StringContent(Request.Language), "language");
            if (Request.Temperature != null)
                content.Add(new StringContent(Request.Temperature.ToString()!), "temperature");

            Request.Dispose();

            var response = await Client.PostAsync<VerboseAudioResult>(Configuration.GetUri(OpenAiType.AudioTranscription, Request.Model!, _forced, string.Empty), content, Configuration, cancellationToken);
            return response;
        }
        /// <summary>
        /// Translates audio into English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>AudioResult</returns>
        public async ValueTask<AudioResult> TranslateAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatJson;
            using var content = new MultipartFormDataContent();
            if (Request.Audio != null)
                content.Add(new ByteArrayContent(Request.Audio.ToArray()), "file", Request.AudioName ?? Guid.NewGuid().ToString());
            if (Request.Model != null)
                content.Add(new StringContent(Request.Model.ToString()), "model");
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), "prompt");
            if (Request.ResponseFormat != null)
                content.Add(new StringContent(Request.ResponseFormat), "response_format");
            if (Request.Temperature != null)
                content.Add(new StringContent(Request.Temperature.ToString()!), "temperature");

            Request.Dispose();

            var response = await Client.PostAsync<AudioResult>(Configuration.GetUri(OpenAiType.AudioTranslation, Request.Model!, _forced, string.Empty), content, Configuration, cancellationToken);
            return response;
        }
        /// <summary>
        /// Translates audio into a verbose representation in English.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>VerboseAudioResult</returns>
        public async ValueTask<VerboseAudioResult> VerboseTranslateAsync(CancellationToken cancellationToken = default)
        {
            Request.ResponseFormat = ResponseFormatVerboseJson;
            using var content = new MultipartFormDataContent();
            if (Request.Audio != null)
                content.Add(new ByteArrayContent(Request.Audio.ToArray()), "file", Request.AudioName ?? Guid.NewGuid().ToString());
            if (Request.Model != null)
                content.Add(new StringContent(Request.Model.ToString()), "model");
            if (Request.Prompt != null)
                content.Add(new StringContent(Request.Prompt), "prompt");
            if (Request.ResponseFormat != null)
                content.Add(new StringContent(Request.ResponseFormat), "response_format");
            if (Request.Temperature != null)
                content.Add(new StringContent(Request.Temperature.ToString()!), "temperature");

            Request.Dispose();

            var response = await Client.PostAsync<VerboseAudioResult>(Configuration.GetUri(OpenAiType.AudioTranslation, Request.Model!, _forced, string.Empty), content, Configuration, cancellationToken);
            return response;
        }
        /// <summary>
        /// An optional text to guide the model's style or continue a previous audio segment. The prompt should match the audio language.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns>AudioRequestBuilder</returns>
        public AudioRequestBuilder WithPrompt(string prompt)
        {
            Request.Prompt = prompt;
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
            Request.Temperature = temperature;
            return this;
        }
        /// <summary>
        /// The language of the input audio. Supplying the input language in <see href="https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes">ISO-639-1</see> format will improve accuracy and latency.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public AudioRequestBuilder WithLanguage(Language language)
        {
            Request.Language = language.ToIso639_1();
            return this;
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCostForTranscription(int minutes = 0)
        {
            var cost = Utility.Cost;
            return cost.Configure(settings =>
            {
                settings
                    .WithType(OpenAiType.AudioTranscription);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                Minutes = minutes
            });
        }
        /// <summary>
        /// Calculate the cost for this request based on configurated price during startup.
        /// </summary>
        /// <returns>decimal</returns>
        public decimal CalculateCostForTranslation(int minutes = 0)
        {
            var cost = Utility.Cost;
            return cost.Configure(settings =>
            {
                settings
                    .WithType(OpenAiType.AudioTranslation);
            }, Configuration.Name).Invoke(new OpenAiUsage
            {
                Minutes = minutes
            });
        }
    }
}
