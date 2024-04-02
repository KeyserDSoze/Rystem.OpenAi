namespace System.Text.Json.Serialization
{
    public interface ITool
    {
        [JsonPropertyName("type")]
        string Type { get; }
    }
}
