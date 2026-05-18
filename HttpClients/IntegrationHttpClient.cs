using Calls.Entities.Json;
using Calls.HttpClients.Options;
using Calls.Model.Ats.Beeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions.Cases;
using Shared.HttpClients.Base;
using Shared.HttpClients.Options.Base;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public class IntegrationHttpClient : HttpClientBase, IIntegrationHttpClient
    {
        private readonly IntegrationApiOptions options;
        private readonly ClientOptions clientOptions;
        private readonly ClientData data;

        public IntegrationHttpClient(HttpClient client, ClientDataProvider dataProvider, IOptions<ClientOptions> clientOptions, IServiceProvider sp, IOptions<IntegrationApiOptions> options) : base(client, sp, options)
        {
            this.options = options.Value;
            this.clientOptions = clientOptions.Value;
            this.data = dataProvider.Data;
        }

        public async Task<string> SendMessage(IntegrationMessage message, CancellationToken token)
        {
            var result = await PostAsync<string>(message, new MethodArgs
            {
                Method = options.SendMessage.With(("url", data.IntegrationData.Url)),
                QueryArgsType = QueryArgsType.JsonBody,
                GetBasicAuth = () => (data.IntegrationData.Username, data.IntegrationData.Password),
                UseThrowCase = HttpClientCase.Integration,
                CancellationToken = token
            });

            return result;
        }
    }
}
