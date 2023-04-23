using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rystem.OpenAi
{
    public abstract class Results<T> : ApiBaseResponse
        where T : IStatedResult
    {
        [JsonPropertyName("data")]
        public List<T>? Data { get; set; }
        [JsonPropertyName("pendings")]
        public IEnumerable<T> Pendings => Data.Where(x => x.State == EventState.Pending);
        [JsonPropertyName("notRunnings")]
        public IEnumerable<T> NotRunnings => Data.Where(x => x.State == EventState.NotRunning);
        [JsonPropertyName("runnings")]
        public IEnumerable<T> Runnings => Data.Where(x => x.State == EventState.Running);
        [JsonPropertyName("succeeded")]
        public IEnumerable<T> Succeeded => Data.Where(x => x.State == EventState.Succeeded);
        [JsonPropertyName("failed")]
        public IEnumerable<T> Failed => Data.Where(x => x.State == EventState.Failed);
        [JsonPropertyName("cancelled")]
        public IEnumerable<T> Cancelled => Data.Where(x => x.State == EventState.Canceled);
        [JsonPropertyName("deleted")]
        public IEnumerable<T> Deleted => Data.Where(x => x.State == EventState.Deleted);
    }
}
