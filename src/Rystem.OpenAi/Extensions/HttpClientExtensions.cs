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
        private sealed class ErrorResponse
        {
            [JsonPropertyName("error")]
            public required string Error { get; set; }
            [JsonPropertyName("request")]
            public required string Request { get; set; }
            [JsonPropertyName("method")]
            public required string Method { get; set; }
            [JsonPropertyName("streaming")]
            public required bool Streaming { get; set; }
            [JsonPropertyName("content")]
            public object? Content { get; set; }
            public override string ToString()
            {
                return $"""
                
                Error: {Error}
                Request: {Request}
                Method: {Method}
                Streaming: {Streaming}
                Content: {Content?.ToJson(s_options)}
                """;
            }
        }
        private static readonly JsonSerializerOptions s_options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        private static async Task<HttpResponseMessage> PerformRequestAsync(this HttpClientWrapper wrapper,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
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
                var error = new ErrorResponse
                {
                    Error = errorMessage,
                    Request = url,
                    Method = method.Method,
                    Streaming = isStreaming,
                    Content = message
                };
                throw new HttpRequestException(error.ToString());
            }

        }
        internal static async Task<HttpResponseMessage> ExecuteAsync(this HttpClientWrapper wrapper,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(wrapper);
            return await PerformRequestAsync(wrapper, url, method, message, isStreaming, cancellationToken);
        }
        internal static ValueTask<TResponse> DeleteAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, null, HttpMethod.Delete, headers, configuration, cancellationToken);
        internal static ValueTask<TResponse> GetAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, null, HttpMethod.Get, headers, configuration, cancellationToken);
        internal static ValueTask<TResponse> PatchAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Patch, headers, configuration, cancellationToken);
        internal static ValueTask<TResponse> PutAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Put, headers, configuration, cancellationToken);
        internal static ValueTask<TResponse> PostAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteWithResponseAsync<TResponse>(wrapper, url, message, HttpMethod.Post, headers, configuration, cancellationToken);
        internal static async ValueTask<TResponse> ExecuteWithResponseAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod method,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
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
            var response = await wrapper.PerformRequestAsync(url, method, message, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken);
            return !string.IsNullOrWhiteSpace(responseAsString) ? JsonSerializer.Deserialize<TResponse>(responseAsString)! : default!;
        }
        internal static ValueTask<Stream> PostAsync(this HttpClientWrapper wrapper,
            string url,
            object? message,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
            => ExecuteAndResponseAsStreamAsync(wrapper, url, message, HttpMethod.Post, headers, configuration, cancellationToken);
        internal static async ValueTask<Stream> ExecuteAndResponseAsStreamAsync(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod method,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
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
            var response = await wrapper.PerformRequestAsync(url, method, message, false, cancellationToken);
            var responseAsStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var memoryStream = new MemoryStream();
            await responseAsStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
        internal static async IAsyncEnumerable<TResponse> StreamAsync<TResponse>(this HttpClientWrapper wrapper,
            string url,
            object? message,
            HttpMethod httpMethod,
            Dictionary<string, string>? headers,
            OpenAiConfiguration configuration,
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
            var response = await wrapper.PerformRequestAsync(url, httpMethod, message, true, cancellationToken);
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var items = streamReader != null ? streamReader(stream, response, cancellationToken) : DefaultStreamReader<TResponse>(stream, response, cancellationToken);
            await foreach (var item in items)
                yield return item!;
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
