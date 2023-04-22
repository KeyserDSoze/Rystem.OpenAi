using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rystem.OpenAi.Management
{
    public sealed class BillingBuilder : RequestBuilder<BillingRequest>
    {
        public BillingBuilder(HttpClient client, OpenAiConfiguration configuration, IOpenAiUtility utility) :
            base(client, configuration, () =>
            {
                return new BillingRequest()
                {
                    From = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, 0),
                    To = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1, 0, 0, 0, 0).AddDays(-1)
                };
            }, utility)
        {

        }
        public BillingBuilder From(DateTime from)
        {
            Request.From = from;
            return this;
        }
        public BillingBuilder To(DateTime to)
        {
            Request.To = to;
            return this;
        }
        public ValueTask<BillingResult> GetUsageAsync(CancellationToken cancellationToken = default)
        {
#warning it doesn't with Azure
            var uri = $"{Configuration.GetUri(OpenAiType.Billing, string.Empty, false, string.Empty)}?end_date={Request.To:yyyy-MM-dd}&start_date={Request.From:yyyy-MM-dd}";
            return Client.GetAsync<BillingResult>(uri, Configuration, cancellationToken);
        }
    }
}
