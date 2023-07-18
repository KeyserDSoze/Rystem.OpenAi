using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Test.Functions
{
    internal sealed class NullRequestModel
    {
        [JsonPropertyName("username")]
        [JsonPropertyDescription("The username of the user.")]
        [JsonRequired]
        public string Username { get; set; }
    }
}
