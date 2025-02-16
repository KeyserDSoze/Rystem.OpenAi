using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.RealTime
{
    /// <summary>
    /// Encapsulates a WebSocket connection to the Realtime API and provides helper methods to send events.
    /// </summary>
    public sealed class RealTimeClient : IDisposable, IAsyncDisposable
    {
        private readonly Uri _uri;
        private readonly ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts;
        private readonly Dictionary<string, List<Action<RealTimeClientEvent>>> _eventHandlers;

        public RealTimeClient(string url, string token)
        {
            _uri = new Uri($"{url}{(url.Contains('?') ? "&" : "?")}token={Uri.EscapeDataString(token)}");
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
            byte[] buffer = new byte[8192];
            while (!cancellationToken.IsCancellationRequested)
            {
                StringBuilder sb = new StringBuilder();
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

                string message = sb.ToString();
                try
                {
                    // For demonstration, deserialize into the base RealtimeEvent.
                    RealTimeClientEvent evt = JsonSerializer.Deserialize<RealTimeClientEvent>(message);
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
                _eventHandlers[eventType] = new List<Action<RealTimeClientEvent>>();
            _eventHandlers[eventType].Add(handler);
        }

        /// <summary>
        /// Unregisters an event handler.
        /// </summary>
        public void Off(string eventType, Action<RealTimeClientEvent> handler)
        {
            if (_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType].Remove(handler);
        }

        /// <summary>
        /// Sends an event (as a JSON payload) to the server.
        /// </summary>
        public async Task SendEventAsync(object evt)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                Console.WriteLine("WebSocket is not open.");
                return;
            }
            string json = JsonSerializer.Serialize(evt, new JsonSerializerOptions { IgnoreNullValues = true });
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Generates a random event ID.
        /// </summary>
        private string GenerateEventId() => "evt_" + Guid.NewGuid().ToString("N").Substring(0, 8);

        // ─── HELPER METHODS TO SEND CLIENT EVENTS ─────────────────────

        public async Task SessionUpdateAsync(RealTimeClientSessionUpdateData session, string eventId = null)
        {
            RealTimeClientSessionUpdateEvent evt = new RealTimeClientSessionUpdateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "session.update",
                Session = session
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferAppendAsync(string audio, string eventId = null)
        {
            RealTimeClientInputAudioBufferAppendEvent evt = new RealTimeClientInputAudioBufferAppendEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.append",
                Audio = audio
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferCommitAsync(string eventId = null)
        {
            RealTimeClientInputAudioBufferCommitEvent evt = new RealTimeClientInputAudioBufferCommitEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.commit"
            };
            await SendEventAsync(evt);
        }

        public async Task InputAudioBufferClearAsync(string eventId = null)
        {
            RealTimeClientInputAudioBufferClearEvent evt = new RealTimeClientInputAudioBufferClearEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "input_audio_buffer.clear"
            };
            await SendEventAsync(evt);
        }

        public async Task ConversationItemCreateAsync(string previousItemId, RealTimeClientConversationItem item, string eventId = null)
        {
            RealTimeClientConversationItemCreateEvent evt = new RealTimeClientConversationItemCreateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "conversation.item.create",
                PreviousItemId = previousItemId,
                Item = item
            };
            await SendEventAsync(evt);
        }

        public async Task ConversationItemTruncateAsync(string itemId, int contentIndex, int audioEndMs, string eventId = null)
        {
            RealTimeClientConversationItemTruncateEvent evt = new RealTimeClientConversationItemTruncateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "conversation.item.truncate",
                ItemId = itemId,
                ContentIndex = contentIndex,
                AudioEndMs = audioEndMs
            };
            await SendEventAsync(evt);
        }

        public async Task ConversationItemDeleteAsync(string itemId, string eventId = null)
        {
            RealTimeClientConversationItemDeleteEvent evt = new RealTimeClientConversationItemDeleteEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "conversation.item.delete",
                ItemId = itemId
            };
            await SendEventAsync(evt);
        }

        public async Task ResponseCreateAsync(RealTimeClientResponseCreateData response, string eventId = null)
        {
            RealTimeClientResponseCreateEvent evt = new RealTimeClientResponseCreateEvent
            {
                EventId = eventId ?? GenerateEventId(),
                Type = "response.create",
                Response = response
            };
            await SendEventAsync(evt);
        }

        public async Task ResponseCancelAsync(string responseId = null, string eventId = null)
        {
            RealTimeClientResponseCancelEvent evt = new RealTimeClientResponseCancelEvent
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
