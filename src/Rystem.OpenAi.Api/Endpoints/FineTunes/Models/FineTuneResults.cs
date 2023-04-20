using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneResults : ApiBaseResponse
    {
        [JsonPropertyName("data")]
        public List<FineTuneResult>? Data { get; set; }
        [JsonPropertyName("pendings")]
        public IEnumerable<FineTuneResult> Pendings => Data.Where(x => x.Status == Pending);
        [JsonPropertyName("runnings")]
        public IEnumerable<FineTuneResult> Runnings => Data.Where(x => x.Status == Run);
        [JsonPropertyName("succeeded")]
        public IEnumerable<FineTuneResult> Succeeded => Data.Where(x => x.Status == Success);
        [JsonPropertyName("failed")]
        public IEnumerable<FineTuneResult> Failed => Data.Where(x => x.Status == Fail);
        [JsonPropertyName("cancelled")]
        public IEnumerable<FineTuneResult> Cancelled => Data.Where(x => x.Status == Cancel);
        private const string Pending = "pending";
        private const string Run = "running";
        private const string Success = "succeeded";
        private const string Fail = "failed";
        private const string Cancel = "cancelled";
    }
}
