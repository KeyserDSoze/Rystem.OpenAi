﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResults : ResponseAsArray<FineTuneResult>
    {
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? ValidatingFiles => Data?.Where(x => x.Status == FineTuneStatus.ValidatingFiles);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? NotRunnings => Data?.Where(x => x.Status != FineTuneStatus.Running);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? Runnings => Data?.Where(x => x.Status == FineTuneStatus.Running);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? Succeeded => Data?.Where(x => x.Status == FineTuneStatus.Succeeded);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? Failed => Data?.Where(x => x.Status == FineTuneStatus.Failed);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? Cancelled => Data?.Where(x => x.Status == FineTuneStatus.Cancelled);
        [JsonIgnore]
        public IEnumerable<FineTuneResult>? Queued => Data?.Where(x => x.Status == FineTuneStatus.Queued);
    }
}
