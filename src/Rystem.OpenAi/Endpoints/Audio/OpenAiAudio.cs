using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiAudio : OpenAiBuilder<IOpenAiAudio, AudioRequest, AudioModelName>, IOpenAiAudio
    {
        public OpenAiAudio(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory)
            : base(factory, configurationFactory, OpenAiType.AudioTranscription, OpenAiType.AudioTranslation)
        {
            Request.Model = AudioModelName.Whisper;
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.Audio != null)
            {
                configuration.Settings.DefaultRequestConfiguration.Audio.Invoke(this);
            }
        }
        internal const string ResponseFormatJson = "json";
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

            var response = await DefaultServices.HttpClient.PostAsync<AudioResult>(DefaultServices.Configuration.GetUri(OpenAiType.AudioTranscription, Request.Model!, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
            return response;
        }
        internal const string ResponseFormatVerboseJson = "verbose_json";
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

            var response = await DefaultServices.HttpClient.PostAsync<VerboseAudioResult>(DefaultServices.Configuration.GetUri(OpenAiType.AudioTranscription, Request.Model!, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
            return response;
        }
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

            var response = await DefaultServices.HttpClient.PostAsync<AudioResult>(DefaultServices.Configuration.GetUri(OpenAiType.AudioTranslation, Request.Model!, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
            return response;
        }
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

            var response = await DefaultServices.HttpClient.PostAsync<VerboseAudioResult>(DefaultServices.Configuration.GetUri(OpenAiType.AudioTranslation, Request.Model!, Forced, string.Empty), content, DefaultServices.Configuration, cancellationToken);
            return response;
        }
        public IOpenAiAudio WithFile(byte[] file, string fileName = "default")
        {
            var memoryStream = new MemoryStream(file);
            memoryStream.Seek(0, SeekOrigin.Begin);
            Request.Audio = memoryStream;
            Request.AudioName = fileName;
            return this;
        }
        public async Task<IOpenAiAudio> WithStreamAsync(Stream file, string fileName = "default")
        {
            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream).NoContext();
            memoryStream.Seek(0, SeekOrigin.Begin);
            Request.Audio = memoryStream;
            Request.AudioName = fileName;
            return this;
        }
        public IOpenAiAudio WithPrompt(string prompt)
        {
            Request.Prompt = prompt;
            return this;
        }
        public IOpenAiAudio WithTemperature(double temperature)
        {
            if (temperature < 0)
                throw new ArgumentException("Temperature with a value lesser than 0");
            if (temperature > 1)
                throw new ArgumentException("Temperature with a value greater than 1");
            Request.Temperature = temperature;
            return this;
        }
        public IOpenAiAudio WithLanguage(Language language)
        {
            Request.Language = language.ToIso639_1();
            return this;
        }
        public IOpenAiAudio WithTranscriptionMinutes(int minutes)
        {
            Usages.Add(new OpenAiCost { Units = minutes, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Minutes });
            return this;
        }
        public IOpenAiAudio WithTranslationMinutes(int minutes)
        {
            Usages.Add(new OpenAiCost { Units = minutes, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Minutes });
            return this;
        }
    }
}
