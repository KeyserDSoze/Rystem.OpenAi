using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationCategoryAppliedInputType
    {
        [JsonPropertyName("harassment")]
        public List<string>? Harassment { get; set; }

        [JsonPropertyName("harassment/threatening")]
        public List<string>? HarassmentThreatening { get; set; }

        [JsonPropertyName("sexual")]
        public List<string>? Sexual { get; set; }

        [JsonPropertyName("hate")]
        public List<string>? Hate { get; set; }

        [JsonPropertyName("hate/threatening")]
        public List<string>? HateThreatening { get; set; }

        [JsonPropertyName("illicit")]
        public List<string>? Illicit { get; set; }

        [JsonPropertyName("illicit/violent")]
        public List<string>? IllicitViolent { get; set; }

        [JsonPropertyName("self-harm/intent")]
        public List<string>? SelfHarmIntent { get; set; }

        [JsonPropertyName("self-harm/instructions")]
        public List<string>? SelfHarmInstructions { get; set; }

        [JsonPropertyName("self-harm")]
        public List<string>? SelfHarm { get; set; }

        [JsonPropertyName("sexual/minors")]
        public List<string>? SexualMinors { get; set; }

        [JsonPropertyName("violence")]
        public List<string>? Violence { get; set; }

        [JsonPropertyName("violence/graphic")]
        public List<string>? ViolenceGraphic { get; set; }
    }
}
