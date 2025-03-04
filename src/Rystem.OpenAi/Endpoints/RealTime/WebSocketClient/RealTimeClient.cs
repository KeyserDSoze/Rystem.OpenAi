using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.RealTime
{
    //todo: remove public from classes and setup the interfaces, implements all kind of messages, check for Role to use the Role as the chat endpoint
    //todo: check the json integration with options that should come from the standard nullable json serialization
    public sealed class RealTimeConversationItemBuilder
    {
        private const string ConversationItemCreateType = "conversation.item.create";
        private const string MessageType = "message";
        private const string RealTimeItemObject = "realtime.item";
        private const string InputTextType = "input_text";
        private readonly RealTimeClient _client;
        private readonly string? _previousItemId;
        private readonly string? _eventId;
        private readonly RealTimeClientConversationItem _item;
        public RealTimeConversationItemBuilder(RealTimeClient client, string? previousItemId, string? eventId = null)
        {
            _client = client;
            _previousItemId = previousItemId;
            _eventId = eventId ?? RealTimeClient.GenerateEventId();
            _item = new();
        }
        public RealTimeConversationItemBuilder WithUserMessage(string text)
        {
            _item.Type = MessageType;
            _item.Object = RealTimeItemObject;
            _item.Role = "user";
            _item.Content =
            [
                new RealTimeClientConversationContent
                {
                    Type = InputTextType,
                    Text = text
                }
            ];
            return this;
        }
        public async ValueTask SendAsync()
        {
            var evt = new RealTimeClientConversationItemCreateEvent
            {
                EventId = _eventId,
                Type = ConversationItemCreateType,
                PreviousItemId = _previousItemId,
                Item = _item
            };
            await _client.SendEventAsync(evt);
        }
    }
    /// <summary>
    /// Encapsulates a WebSocket connection to the Realtime API and provides helper methods to send events.
    /// </summary>
    public sealed class RealTimeClient : IDisposable, IAsyncDisposable
    {
        private const string AndQueryString = "&";
        private const string QuestionMarkQueryString = "?";
        private readonly Uri _uri;
        private readonly ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts;
        private readonly Dictionary<string, List<Action<RealTimeClientEvent>>> _eventHandlers;

        public RealTimeClient(string url, string? apiKey, string? token)
        {
            if (apiKey == null && token == null)
                throw new ArgumentException("Either apiKey or token must be provided.");
            _uri = new Uri($"{url}{(url.Contains('?') ? AndQueryString : QuestionMarkQueryString)}{(token == null ? $"api-key={Uri.EscapeDataString(apiKey!)}" : $"token={Uri.EscapeDataString(token)}")}");
            _webSocket = new ClientWebSocket();
            _eventHandlers = [];
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Connects to the WebSocket endpoint.
        /// </summary>
        public async Task ConnectAsync()
        {
            await _webSocket.ConnectAsync(_uri, _cts.Token);
            Console.WriteLine("Connected to Realtime WebSocket.");
            _ = ReceiveLoopAsync(_cts.Token); // Fire-and-forget.
        }

        /// <summary>
        /// Disconnects from the WebSocket.
        /// </summary>
        public async Task DisconnectAsync()
        {
            _cts.Cancel();
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        /// <summary>
        /// Receives messages continuously and dispatches them to event handlers.
        /// </summary>
        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            while (!cancellationToken.IsCancellationRequested)
            {
                var sb = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
                        return;
                    }
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                var message = sb.ToString();
                try
                {
                    // For demonstration, deserialize into the base RealtimeEvent.
                    var evt = message.FromJson<RealTimeClientEvent>(HttpClientWrapperExtensions.JsonSerializationOptions);
                    DispatchEvent(evt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing message: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Dispatches a received event to all registered handlers.
        /// </summary>
        private void DispatchEvent(RealTimeClientEvent evt)
        {
            if (evt == null || string.IsNullOrEmpty(evt.Type))
                return;

            if (_eventHandlers.TryGetValue(evt.Type, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler(evt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in event handler: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Registers an event handler for a given event type.
        /// </summary>
        public void On(string eventType, Action<RealTimeClientEvent> handler)
        {
            if (!_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType] = [];
            _eventHandlers[eventType].Add(handler);
        }

        /// <summary>
        /// Unregisters an event handler.
        /// </summary>
        public void Off(string eventType, Action<RealTimeClientEvent> handler)
        {
            if (_eventHandlers.TryGetValue(eventType, out var value))
                value.Remove(handler);
        }

        /// <summary>
        /// Sends an event (as a JSON payload) to the server.
        /// </summary>
        internal async Task SendEventAsync(object evt)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                Console.WriteLine("WebSocket is not open.");
                return;
            }
            var json = evt.ToJson(HttpClientWrapperExtensions.JsonSerializationOptions);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Generates a random event ID.
        /// </summary>
        internal static string GenerateEventId() => string.Concat("evt_", Guid.NewGuid().ToString("N").AsSpan(0, 8));

        public async Task SessionUpdateAsync(RealTimeClientSessionUpdateData session, string? eventId = null)
        {
            var evt = new RealTimeClientSessionUpdateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "session.update",
                Session = session
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferAppendAsync(byte[] audio, string? eventId = null)
        {
            var evt = new RealTimeClientInputAudioBufferAppendEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.append",
                Audio = audio.ToBase64()
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferCommitAsync(string? eventId = null)
        {
            var evt = new RealTimeClientInputAudioBufferCommitEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.commit"
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferClearAsync(string? eventId = null)
        {
            var evt = new RealTimeClientInputAudioBufferClearEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.clear"
            };
            await SendEventAsync(evt);
        }

        public RealTimeConversationItemBuilder ConversationItemCreate(string? previousItemId, string? eventId = null)
            => new(this, previousItemId, eventId);

        public async Task ConversationItemTruncateAsync(string itemId, int contentIndex, int audioEndMs, string? eventId = null)
        {
            var evt = new RealTimeClientConversationItemTruncateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "conversation.item.truncate",
                ItemId = itemId,
                ContentIndex = contentIndex,
                AudioEndMs = audioEndMs
            };
            await SendEventAsync(evt);
        }

        public async Task ConversationItemDeleteAsync(string itemId, string? eventId = null)
        {
            var evt = new RealTimeClientConversationItemDeleteEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "conversation.item.delete",
                ItemId = itemId
            };
            await SendEventAsync(evt);
        }

        public async Task ResponseCreateAsync(RealTimeClientResponseCreateData response, string? eventId = null)
        {
            var evt = new RealTimeClientResponseCreateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "response.create",
                Response = response
            };
            await SendEventAsync(evt);
        }

        public async Task ResponseCancelAsync(string responseId, string? eventId = null)
        {
            var evt = new RealTimeClientResponseCancelEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "response.cancel",
                ResponseId = responseId
            };
            await SendEventAsync(evt);
        }
        private bool _disposed = false;
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
                await DisconnectAsync();
            _disposed = true;
        }
        public void Dispose()
        {
            if (!_disposed)
                DisconnectAsync().ToResult();
            _disposed = true;
        }
    }
}
