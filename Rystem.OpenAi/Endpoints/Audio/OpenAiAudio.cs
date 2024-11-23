﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.Audio
{
    internal sealed class OpenAiAudio : OpenAiBuilder<IOpenAiAudio, AudioRequest, ChatUsage>, IOpenAiAudio
    {
        public OpenAiAudio(IFactory<DefaultServices> factory)
            : base(factory)
        {
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
        public decimal CalculateCostForTranscription(int minutes = 0)
        {
            decimal outputPrice = 0;
            foreach (var responses in Usages)
            {
                outputPrice += DefaultServices.Price.CalculatePrice(Request.Model!,
                    new OpenAiCost { Units = minutes, Kind = KindOfCost.Input, UnitOfMeasure = UnitOfMeasure.Minutes });
            }
            return outputPrice;
        }
        public decimal CalculateCostForTranslation(int minutes = 0)
        {
            //todo: to refactor the audio input choose i don't like it
            decimal outputPrice = 0;
            foreach (var responses in Usages)
            {
                outputPrice += DefaultServices.Price.CalculatePrice(Request.Model!,
                    new OpenAiCost { Units = minutes, Kind = KindOfCost.AudioInput, UnitOfMeasure = UnitOfMeasure.Minutes });
            }
            return outputPrice;
        }
    }
}