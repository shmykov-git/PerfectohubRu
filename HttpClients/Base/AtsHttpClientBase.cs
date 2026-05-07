using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.HttpClients.Base;
using Shared.HttpClients.Options.Base;
using Shared.Model.Enums;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calls.HttpClients.Base
{
    public abstract class AtsHttpClientBase<TCallItem> : HttpClientBase
    {
        protected IConfiguration configuration;
        private readonly AtsHttpClientOptions options;

        public abstract AtsType AtsType { get; }
        public virtual string ServiceName => this.GetType().Name;

        protected abstract Task<TCallItem[]> GetHttpDayCalls(DateTime todayUtc);
        protected abstract bool FilterAtsCalls(TCallItem callItem);

        protected AtsHttpClientBase(HttpClient client, IServiceProvider sp, IOptions<AtsHttpClientOptions> options) 
            : base(client, sp, options)
        {
            this.options = options.Value;
            this.configuration = sp.GetRequiredService<IConfiguration>();
        }
    }
}
