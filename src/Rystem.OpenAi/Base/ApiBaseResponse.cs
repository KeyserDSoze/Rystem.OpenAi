﻿using System;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    /// <summary>
    /// Represents a result from calling the OpenAI API, with all the common metadata returned from every endpoint
    /// </summary>
    public abstract class ApiBaseResponse
    {
        /// The time when the result was generated
        [JsonIgnore]
        public DateTime? Created => CreatedUnixTime.HasValue ? (DateTime?)(DateTimeOffset.FromUnixTimeSeconds(CreatedUnixTime.Value).DateTime) : null;
        /// <summary>
        /// The time when the result was generated in unix epoch format
        /// </summary>
        [JsonPropertyName("created")]
        public long? CreatedUnixTime { get; set; }
        /// <summary>
        /// Which model was used to generate this result.
        /// </summary>
        [JsonPropertyName("model")]
        public string? Model { get; set; }
        /// <summary>
        /// Object type, ie: text_completion, file, fine-tune, list, etc
        /// </summary>
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        /// <summary>
        /// The organization associated with the API request, as reported by the API.
        /// </summary>
        [JsonIgnore]
        public string? Organization { get; internal set; }
        /// <summary>
        /// The server-side processing time as reported by the API.  This can be useful for debugging where a delay occurs.
        /// </summary>
        [JsonIgnore]
        public TimeSpan ProcessingTime { get; internal set; }
        /// <summary>
        /// The request id of this API call, as reported in the response headers.  This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
        /// </summary>
        [JsonIgnore]
        public string? RequestId { get; internal set; }
        /// <summary>
        /// The Openai-Version used to generate this response, as reported in the response headers.  This may be useful for troubleshooting or when contacting OpenAI support in reference to a specific request.
        /// </summary>
        [JsonIgnore]
        public string? OpenaiVersion { get; internal set; }
    }
}
