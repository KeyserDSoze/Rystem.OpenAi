using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi
{
    public static class HttpClientWrapperExtensions
    {
        private static readonly JsonSerializerOptions s_options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        private static async Task<HttpResponseMessage> PerformRequestAsync(this HttpClientWrapper wrapper,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(method, url);
            if (message != null)
            {
                if (message is HttpContent httpContent)
                {
                    request.Content = httpContent;
                }
                else
                {
                    var jsonContent = message.ToJson(s_options);
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    request.Content = stringContent;
                }
            }
            var response = wrapper.Policy == null ?
                await wrapper.Client.SendAsync(request, isStreaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken) :
                await wrapper.Policy.ExecuteAsync(ct => wrapper.Client.SendAsync(request, isStreaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, ct), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(errorMessage))
                    errorMessage = $"Status code: {response.StatusCode} with reason: {response.ReasonPhrase}";
                logger
                    .AddError(errorMessage)
                    .LogError();
                throw new HttpRequestException(logger.ToString());
            }
        }
        internal static async Task<HttpResponseMessage> ExecuteAsync(this HttpClientWrapper wrapper,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(wrapper);
            return await PerformRequestAsync(wrapper, url, method, message, isStreaming, logger, cancellationToken);
        }
        internal static ValueTask<TResponse> DeleteAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, null, HttpMethod.Delete, headers, configuration, logger, cancellationToken);
        internal static ValueTask<TResponse> GetAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, null, HttpMethod.Get, headers, configuration, logger, cancellationToken);
        internal static ValueTask<TResponse> PatchAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Patch, headers, configuration, logger, cancellationToken);
        internal static ValueTask<TResponse> PutAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Put, headers, configuration, logger, cancellationToken);
        internal static ValueTask<TResponse> PostAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Post, headers, configuration, logger, cancellationToken);
        internal static async ValueTask<TResponse> ExecuteWithResponseAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod method,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(wrapper);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    wrapper.Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            logger
                .ConfigureUrl(url)
                .ConfigureMethod(method.Method)
                .WithoutStreaming()
                .AddContent(message)
                .StartTimer();
            var response = await wrapper.PerformRequestAsync(url, method, message, false, logger, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                logger
                    .AddResponse(responseAsString)
                    .LogInformation();
                return !string.IsNullOrWhiteSpace(responseAsString) ? JsonSerializer.Deserialize<TResponse>(responseAsString)! : default!;
            }
            catch (Exception ex)
            {
                logger
                    .AddException(ex)
                    .LogError();
                throw new FormatException(logger.ToString(), ex);
            }
        }
        internal static ValueTask<Stream> PostAsync(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
            => ExecuteAndResponseAsStreamAsync(wrapper, url, message, HttpMethod.Post, headers, configuration, logger, cancellationToken);
        internal static async ValueTask<Stream> ExecuteAndResponseAsStreamAsync(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod method,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(wrapper);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    wrapper.Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            logger
                .ConfigureUrl(url)
                .ConfigureMethod(method.Method)
                .WithoutStreaming()
                .AddContent(message)
                .StartTimer();
            var response = await wrapper.PerformRequestAsync(url, method, message, false, logger, cancellationToken);
            var responseAsStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var memoryStream = new MemoryStream();
            await responseAsStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            logger
                .AddResponse(nameof(Stream))
                .LogInformation();
            return memoryStream;
        }
        internal static async IAsyncEnumerable<TResponse> StreamAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod method,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            IOpenAiLogger logger,
            Func<Stream, HttpResponseMessage, CancellationToken, IAsyncEnumerable<TResponse>>? streamReader,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(wrapper);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    wrapper.Client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
            logger
                .ConfigureUrl(url)
                .ConfigureMethod(method.Method)
                .WithStreaming()
                .AddContent(message)
                .StartTimer();
            var response = await wrapper.PerformRequestAsync(url, method, message, true, logger, cancellationToken);
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var items = streamReader != null ? streamReader(stream, response, cancellationToken) : DefaultStreamReader<TResponse>(stream, response, cancellationToken);
            await foreach (var item in items)
            {
                logger
                    .AddResponse(item)
                    .Count()
                    .LogInformation();
                yield return item!;
                logger
                    .StartTimer();
            }
        }
        private static async IAsyncEnumerable<TResponse> DefaultStreamReader<TResponse>(Stream stream, HttpResponseMessage response, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken
                    .ThrowIfCancellationRequested();
                if (line.StartsWith(ChatConstants.Streaming.StartingWith))
                    line = line[ChatConstants.Streaming.StartingWith.Length..];
                if (line == ChatConstants.Streaming.Done)
                {
                    yield break;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    var res = JsonSerializer.Deserialize<TResponse>(line);
                    if (res is ApiBaseResponse apiResult)
                        apiResult.SetHeaders(response);
                    yield return res!;
                }
            }
        }
        internal static void SetHeaders<TResponse>(this TResponse result, HttpResponseMessage response)
            where TResponse : ApiBaseResponse
        {
            try
            {
                response.Headers.TryGetValues("Openai-Organization", out var organizations);
                if (organizations?.Any() == true)
                    result.Organization = organizations.First();
                response.Headers.TryGetValues("X-Request-ID", out var requestIds);
                if (requestIds?.Any() == true)
                    result.RequestId = requestIds.First();
                response.Headers.TryGetValues("Openai-Processing-Ms", out var processing);
                if (processing?.Any() == true)
                    result.ProcessingTime = TimeSpan.FromMilliseconds(double.Parse(processing.First()));
                response.Headers.TryGetValues("Openai-Version", out var versions);
                if (versions?.Any() == true)
                    result.OpenaiVersion = versions.First();
                response.Headers.TryGetValues("Openai-Model", out var models);
                if (models?.Any() == true)
                    result.Model = models.First();
            }
            catch (Exception e)
            {
                Debug.Print($"Issue parsing metadata of OpenAi Response.  Error: {e.Message}.  This is probably ignorable.");
            }
        }
    }
}
