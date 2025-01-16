using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi.FineTune
{
    internal sealed class OpenAiFineTune : OpenAiBuilder<IOpenAiFineTune, FineTuneRequest, FineTuningModelName>, IOpenAiFineTune
    {
        private const string WandBLabel = "wandb";
        public OpenAiFineTune(IFactory<DefaultServices> factory, IFactory<OpenAiConfiguration> configurationFactory, IOpenAiLoggerFactory loggerFactory)
            : base(factory, configurationFactory, loggerFactory, OpenAiType.FineTuning)
        {
            Request.Model = FineTuningModelName.Gpt_4o_2024_08_06;
        }
        private protected override void ConfigureFactory(string name)
        {
            var configuration = ConfigurationFactory.Create(name);
            if (configuration?.Settings?.DefaultRequestConfiguration?.FineTune != null)
            {
                configuration.Settings.DefaultRequestConfiguration.FineTune.Invoke(this);
            }
        }
        public IOpenAiFineTune WithFileId(string trainingFileId)
        {
            Request.TrainingFile = trainingFileId;
            return this;
        }
        public ValueTask<FineTuneResults> ListAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() }
            };
            if (skip > 0)
                querystring.Add("after", skip.ToString());
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, null, string.Empty, querystring);
            return DefaultServices.HttpClientWrapper
                .GetAsync<FineTuneResults>(
                    uri,
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);
        }

        public ValueTask<FineTuneResult> RetrieveAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .GetAsync<FineTuneResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, fineTuneId, $"/{fineTuneId}", null),
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);
        public ValueTask<FineTuneResult> CancelAsync(string fineTuneId, CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .PostAsync<FineTuneResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, fineTuneId, $"/{fineTuneId}/cancel", null),
                    null,
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);
        public ValueTask<ResponseAsArray<FineTuneCheckPointResult>> CheckPointEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() }
            };
            if (skip > 0)
                querystring.Add("after", skip.ToString());
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, fineTuneId, $"/{fineTuneId}/checkpoints", querystring);
            return DefaultServices.HttpClientWrapper
                    .GetAsync<ResponseAsArray<FineTuneCheckPointResult>>(
                        uri,
                        null,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public ValueTask<ResponseAsArray<FineTuneEvent>> ListEventsAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() }
            };
            if (skip > 0)
                querystring.Add("after", skip.ToString());
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, fineTuneId, $"/{fineTuneId}/events", querystring);
            return DefaultServices.HttpClientWrapper
                    .GetAsync<ResponseAsArray<FineTuneEvent>>(
                        uri,
                        null,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        cancellationToken);
        }
        public IAsyncEnumerable<FineTuneResult> ListAsStreamAsync(int take = 20, int skip = 0, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() }
            };
            if (skip > 0)
                querystring.Add("after", skip.ToString());
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, null, string.Empty, querystring);
            return DefaultServices.HttpClientWrapper
                    .StreamAsync(
                        uri,
                        null,
                        HttpMethod.Get,
                        null,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        ReadFineTuneStreamAsync,
                        cancellationToken);
        }

        public IAsyncEnumerable<FineTuneEvent> ListEventsAsStreamAsync(string fineTuneId, int take = 20, int skip = 0, CancellationToken cancellationToken = default)
        {
            var querystring = new Dictionary<string, string>
            {
                { "limit", take.ToString() }
            };
            if (skip > 0)
                querystring.Add("after", skip.ToString());
            var uri = DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, fineTuneId, $"/{fineTuneId}/events", querystring);
            return DefaultServices.HttpClientWrapper
                    .StreamAsync(
                        uri,
                        null,
                        HttpMethod.Get,
                        null,
                        DefaultServices.Configuration,
                        LoggerFactory.Create(),
                        ReadEventsStreamAsync,
                        cancellationToken);
        }

        private static IAsyncEnumerable<FineTuneResult> ReadFineTuneStreamAsync(Stream stream, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return ReadStreamAsync(stream, response, bufferAsString =>
            {
                var chunkResponse = bufferAsString.FromJson<FineTuneResults>();
                var chunk = chunkResponse!.Data?.LastOrDefault();
                return chunk!;
            }, cancellationToken);
        }
        private static IAsyncEnumerable<FineTuneEvent> ReadEventsStreamAsync(Stream stream, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            return ReadStreamAsync(stream, response, bufferAsString =>
            {
                var chunkResponse = bufferAsString.FromJson<ResponseAsArray<FineTuneEvent>>();
                var chunk = chunkResponse!.Data?.LastOrDefault();
                return chunk!;
            }, cancellationToken);
        }
        private static async IAsyncEnumerable<T> ReadStreamAsync<T>(Stream stream, HttpResponseMessage response, Func<string, T> entityReader, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var reader = new StreamReader(stream);
            string? line;
            var buffer = new StringBuilder();
            var curlyCounter = 0;
            var squareCounter = 0;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (line.StartsWith(ChatConstants.Streaming.StartingWith))
                    line = line[ChatConstants.Streaming.StartingWith.Length..];
                if (line == ChatConstants.Streaming.Done)
                {
                    yield break;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    buffer.AppendLine(line);
                    var openCurlyCounter = line.Count(x => x == '{');
                    var openSquareCounter = line.Count(x => x == '[');
                    var closeCurlyCounter = line.Count(x => x == '}');
                    var closeSquareCounter = line.Count(x => x == ']');
                    curlyCounter += openCurlyCounter - closeCurlyCounter;
                    squareCounter += openSquareCounter - closeSquareCounter;
                    if (curlyCounter + squareCounter == 2)
                    {
                        var bufferAsString = $"{buffer.ToString().Trim().Trim(',')}{new string(']', squareCounter)}{new string('}', curlyCounter)}";
                        var chunk = entityReader(bufferAsString);
                        if (chunk != null)
                        {
                            if (chunk is ApiBaseResponse apiResult)
                                apiResult.SetHeaders(response);
                            yield return chunk;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Execute operation.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<FineTuneResult> ExecuteAsync(CancellationToken cancellationToken = default)
            => DefaultServices.HttpClientWrapper
                .PostAsync<FineTuneResult>(
                    DefaultServices.Configuration.GetUri(OpenAiType.FineTuning, _version, Request.TrainingFile!, string.Empty, null),
                    Request,
                    null,
                    DefaultServices.Configuration,
                    LoggerFactory.Create(),
                    cancellationToken);

        /// <summary>
        /// The ID of an uploaded file that contains validation data.
        /// If you provide this file, the data is used to generate validation metrics periodically during fine-tuning. These metrics can be viewed in the <see href="https://platform.openai.com/docs/guides/fine-tuning/analyzing-your-fine-tuned-model">fine-tuning results file</see>. Your train and validation data should be mutually exclusive.
        /// Your dataset must be formatted as a JSONL file, where each validation example is a JSON object with the keys "prompt" and "completion". Additionally, you must upload your file with the purpose fine-tune.
        /// See the <see href="https://platform.openai.com/docs/guides/fine-tuning/creating-training-data">fine-tuning guide</see> for more details.
        /// </summary>
        /// <param name="validationFileId"></param>
        /// <returns></returns>
        public IOpenAiFineTune WithValidationFile(string validationFileId)
        {
            Request.ValidationFile = validationFileId;
            return this;
        }
        /// <summary>
        /// The number of epochs to train the model for. An epoch refers to one full cycle through the training dataset.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IOpenAiFineTune WithHyperParameters(Action<FineTuneHyperParameters> hyperParametersSettings)
        {
            if (Request.Hyperparameters == null)
                Request.Hyperparameters = new FineTuneHyperParameters();
            hyperParametersSettings.Invoke(Request.Hyperparameters);
            return this;
        }
        /// <summary>
        /// A string of up to 40 characters that will be added to your fine-tuned model name.
        /// For example, a suffix of "custom-model-name" would produce a model name like ada:ft-your-org:custom-model-name-2022-02-15-04-21-04.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IOpenAiFineTune WithSuffix(string value)
        {
            Request.Suffix = value;
            return this;
        }
        /// <summary>
        /// The seed controls the reproducibility of the job. Passing in the same seed and job parameters should produce the same results, but may differ in rare cases. If a seed is not specified, one will be generated for you.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public IOpenAiFineTune WithSeed(int seed)
        {
            Request.Seed = seed;
            return this;
        }
        public IOpenAiFineTune WithSpecificWeightAndBiasesIntegration(Action<WeightsAndBiasesFineTuneIntegration> integration)
        {
            Request.Integrations ??= [];
            var wandb = new WeightsAndBiasesFineTuneIntegration();
            integration.Invoke(wandb);
            Request.Integrations.Add(new FineTuneIntegration
            {
                Type = WandBLabel,
                WeightsAndBiases = wandb
            });
            return this;
        }
        public IOpenAiFineTune ClearIntegrations()
        {
            Request.Integrations?.Clear();
            return this;
        }
    }
}
