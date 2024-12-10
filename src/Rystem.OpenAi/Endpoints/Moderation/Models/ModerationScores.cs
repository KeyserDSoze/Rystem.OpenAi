using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationScores
    {
        [JsonPropertyName("harassment")]
        public double Harassment { get; set; }

        [JsonPropertyName("harassment/threatening")]
        public double HarassmentThreatening { get; set; }

        [JsonPropertyName("sexual")]
        public double Sexual { get; set; }

        [JsonPropertyName("hate")]
        public double Hate { get; set; }

        [JsonPropertyName("hate/threatening")]
        public double HateThreatening { get; set; }

        [JsonPropertyName("illicit")]
        public double Illicit { get; set; }

        [JsonPropertyName("illicit/violent")]
        public double IllicitViolent { get; set; }

        [JsonPropertyName("self-harm/intent")]
        public double SelfHarmIntent { get; set; }

        [JsonPropertyName("self-harm/instructions")]
        public double SelfHarmInstructions { get; set; }

        [JsonPropertyName("self-harm")]
        public double SelfHarm { get; set; }

        [JsonPropertyName("sexual/minors")]
        public double SexualMinors { get; set; }

        [JsonPropertyName("violence")]
        public double Violence { get; set; }

        [JsonPropertyName("violence/graphic")]
        public double ViolenceGraphic { get; set; }
    }
}
