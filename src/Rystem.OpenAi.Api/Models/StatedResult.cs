using System;
using System.Text.Json.Serialization;
using Rystem.OpenAi.Extensions;

namespace Rystem.OpenAi
{
    public abstract class StatedResult : ApiBaseResponse
    {
        private const string None = nameof(EventState.None);
        private string _status = None;
        [JsonPropertyName("status")]
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                State = _status.FromString();
            }
        }
        /// <summary>
        /// The status of the event (ie when an upload operation was done: "uploaded")
        /// </summary>
        [JsonPropertyName("state")]
        public EventState State { get; set; }
    }
}
