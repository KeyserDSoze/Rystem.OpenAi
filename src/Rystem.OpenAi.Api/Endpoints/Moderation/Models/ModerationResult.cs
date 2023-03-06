using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Moderation
{
    public sealed class ModerationResult
    {
        [JsonPropertyName("categories")]
        public ModerationCategories? Categories { get; set; }

        [JsonPropertyName("category_scores")]
        public ModerationScores? Scores { get; set; }

        [JsonPropertyName("flagged")]
        public bool Flagged { get; set; }
    }
}
