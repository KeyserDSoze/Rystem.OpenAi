using System;
using System.Text.Json.Serialization;

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
                var enumValue = $"{value[0..1].ToUpper()}{value[1..].ToLower()}";
                State = (EventState)Enum.Parse(typeof(EventState), enumValue);
            }
        }
        /// <summary>
        /// The status of the event (ie when an upload operation was done: "uploaded")
        /// </summary>
        [JsonPropertyName("state")]
        public EventState State { get; set; }
    }
}
