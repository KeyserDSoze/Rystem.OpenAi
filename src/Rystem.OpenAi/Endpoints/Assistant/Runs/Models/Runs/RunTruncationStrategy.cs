using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunTruncationStrategy
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("last_messages")]
        public int? NumberOfLastMessages { get; set; }
        private const string TruncationTypeLastMessages = "last_messages";
        public static RunTruncationStrategy Auto { get; } = new() { Type = "auto" };
        public static RunTruncationStrategy LastMessages(int numberOfMessages) => new() { Type = TruncationTypeLastMessages, NumberOfLastMessages = numberOfMessages };
    }
}
