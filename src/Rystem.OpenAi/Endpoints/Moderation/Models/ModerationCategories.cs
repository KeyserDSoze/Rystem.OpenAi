using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{

    public sealed class ModerationCategories
    {
        [JsonPropertyName("harassment")]
        public bool Harassment { get; set; }

        [JsonPropertyName("harassment/threatening")]
        public bool HarassmentThreatening { get; set; }

        [JsonPropertyName("sexual")]
        public bool Sexual { get; set; }

        [JsonPropertyName("hate")]
        public bool Hate { get; set; }

        [JsonPropertyName("hate/threatening")]
        public bool HateThreatening { get; set; }

        [JsonPropertyName("illicit")]
        public bool Illicit { get; set; }

        [JsonPropertyName("illicit/violent")]
        public bool IllicitViolent { get; set; }

        [JsonPropertyName("self-harm/intent")]
        public bool SelfHarmIntent { get; set; }

        [JsonPropertyName("self-harm/instructions")]
        public bool SelfHarmInstructions { get; set; }

        [JsonPropertyName("self-harm")]
        public bool SelfHarm { get; set; }

        [JsonPropertyName("sexual/minors")]
        public bool SexualMinors { get; set; }

        [JsonPropertyName("violence")]
        public bool Violence { get; set; }

        [JsonPropertyName("violence/graphic")]
        public bool ViolenceGraphic { get; set; }
    }
}
