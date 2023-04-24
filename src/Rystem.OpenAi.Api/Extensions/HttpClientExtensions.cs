using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions s_options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        private static async Task<HttpResponseMessage> PrivatedExecuteAsync(this HttpClient client,
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
                    var jsonContent = JsonSerializer.Serialize(message, s_options);
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    request.Content = stringContent;
                }
            }
            var response = await client.SendAsync(request, isStreaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }
        }
        internal static async Task<HttpResponseMessage> ExecuteAsync(this HttpClient client,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            return await PrivatedExecuteAsync(client, url, method, message, isStreaming, cancellationToken);
        }
        internal static async ValueTask<TResponse> DeleteAsync<TResponse>(this HttpClient client,
            string url,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Delete, null, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return !string.IsNullOrWhiteSpace(responseAsString) ? JsonSerializer.Deserialize<TResponse>(responseAsString)! : default(TResponse)!;
        }
        internal static async ValueTask<TResponse> GetAsync<TResponse>(this HttpClient client,
            string url,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Get, null, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
        }
        internal static async ValueTask<TResponse> PatchAsync<TResponse>(this HttpClient client,
            string url,
            object? message,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            try
            {
                var response = await client.PrivatedExecuteAsync(url, HttpMethod.Patch, message, false, cancellationToken);
                var responseAsString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
            }
            catch (Exception ec)
            {
                var olaf = ec.Message;
                return default(TResponse);
            }
        }
        internal static async ValueTask<TResponse> PostAsync<TResponse>(this HttpClient client,
            string url,
            object? message,
            OpenAiConfiguration configuration,
            CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Post, message, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
        }
        private const string StartingWith = "data: ";
        private const string Done = "[DONE]";
        internal static async IAsyncEnumerable<TResponse> StreamAsync<TResponse>(this HttpClient client,
            string url,
            object? message,
            HttpMethod httpMethod,
            OpenAiConfiguration configuration,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (configuration.NeedClientEnrichment)
                await configuration.EnrichClientAsync(client);
            var response = await client.PrivatedExecuteAsync(url, httpMethod, message, true, cancellationToken);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith(StartingWith))
                    line = line[StartingWith.Length..];
                if (line == Done)
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
        private static void SetHeaders<TResponse>(this TResponse result, HttpResponseMessage response)
            where TResponse : ApiBaseResponse
        {
            try
            {
                result.Organization = response.Headers.GetValues("Openai-Organization").FirstOrDefault();
                result.RequestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
                result.ProcessingTime = TimeSpan.FromMilliseconds(int.Parse(response.Headers.GetValues("Openai-Processing-Ms").First()));
                result.OpenaiVersion = response.Headers.GetValues("Openai-Version").FirstOrDefault();
                result.ModelId = response.Headers.GetValues("Openai-Model").FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.Print($"Issue parsing metadata of OpenAi Response.  Error: {e.Message}.  This is probably ignorable.");
            }
        }
    }
}
