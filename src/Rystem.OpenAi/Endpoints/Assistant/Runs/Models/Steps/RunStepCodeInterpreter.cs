using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.Assistant
{
    public sealed class RunStepCodeInterpreter
    {
        [JsonPropertyName("input")]
        public string? Input { get; set; }

        [JsonPropertyName("outputs")]
        public List<AnyOf<RunStepCodeInterpreterLogOutput, RunStepCodeInterpreterImageOutput>>? Outputs { get; set; }
    }
}
